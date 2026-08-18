using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace ksimb_membership.Modules.Members;

public interface IMembersService
{
    public Task<List<Member>> GetAllMembers();

    public Task<Member?> GetMemberById(Guid id);

    public Task<Member?> GetMemberByPersonalId(string oib);

    public Task<Member> AddMember(Member member);

    public Task<Guid?> DeleteMember(Guid id);

    public Task<Member?> UpdateMembership(Guid id, MembershipStatus membershipStatus);

    public Task<Member?> UpdateAdminStatus(Guid id, bool status);

    Task<byte[]> ExportMembers();
}

internal sealed class MembersService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory) : IMembersService
{
    public async Task<List<Member>> GetAllMembers()
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.Members.ToListAsync();
    }

    public async Task<Member?> GetMemberById(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var member = await context.Members.FirstOrDefaultAsync(e => e.Id == id);
        if (member is not null && member.CreatedAt < DateTime.Now.AddYears(-1))
        {
            member = await UpdateMembership(member.Id, MembershipStatus.Denied);
        }

        return member;
    }

    public async Task<Member?> GetMemberByPersonalId(string oib)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var member = await context.Members.FirstOrDefaultAsync(e => e.PersonalIdentityNumber == oib);
        if (member is not null && member.CreatedAt < DateTime.Now.AddYears(-1))
        {
            member = await UpdateMembership(member.Id, MembershipStatus.Denied);
        }

        return member;
    }

    public async Task<Member> AddMember(Member member)
    {
        var existing = await GetMemberByPersonalId(member.PersonalIdentityNumber);
        if (existing is not null)
        {
            return existing;
        }

        await using var context = await dbContextFactory.CreateDbContextAsync();

        context.Members.Add(member);
        await context.SaveChangesAsync();
        return member;
    }

    public async Task<Guid?> DeleteMember(Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var member = await GetMemberById(id);
        if (member is null)
        {
            return null;
        }

        context.Members.Remove(member);
        await context.SaveChangesAsync();
        return member.Id;
    }

    public async Task<Member?> UpdateMembership(Guid id, MembershipStatus membershipStatus)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var member = await GetMemberById(id);
        if (member is null)
        {
            return null;
        }

        member.Status = membershipStatus;
        context.Members.Update(member);
        await context.SaveChangesAsync();

        return member;
    }

    public async Task<Member?> UpdateAdminStatus(Guid id, bool status)
    {
        var member = await GetMemberById(id);
        if (member is null)
        {
            return null;
        }


        member.IsAdmin = status;
        await using var context = await dbContextFactory.CreateDbContextAsync();
        context.Members.Update(member);
        await context.SaveChangesAsync();
        return member;
    }

    public async Task<byte[]> ExportMembers()
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var members = await db.Members
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync();

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Članovi");

        worksheet.Cell(1, 1).Value = "Ime";
        worksheet.Cell(1, 2).Value = "Prezime";
        worksheet.Cell(1, 3).Value = "OIB";
        worksheet.Cell(1, 4).Value = "Email";
        worksheet.Cell(1, 5).Value = "Telefon";
        worksheet.Cell(1, 6).Value = "Fakultet";
        worksheet.Cell(1, 7).Value = "Datum rođenja";
        
        var row = 2;

        foreach (var member in members)
        {
            worksheet.Cell(row, 1).Value = member.FirstName;
            worksheet.Cell(row, 2).Value = member.LastName;
            worksheet.Cell(row, 3).Value = member.PersonalIdentityNumber;
            worksheet.Cell(row, 4).Value = member.Email;
            worksheet.Cell(row, 5).Value = member.PhoneNumber;
            worksheet.Cell(row, 6).Value = member.College.ToString();
            worksheet.Cell(row, 7).Value =
                member.DateOfBirth.ToString("dd.MM.yyyy.");

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
    }
}