using Microsoft.AspNetCore.SignalR;
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

        member.CreatedAt = DateTime.Now;
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
}