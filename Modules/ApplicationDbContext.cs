using ksimb_membership.Modules.Members;
using Microsoft.EntityFrameworkCore;

namespace ksimb_membership.Modules;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members => Set<Member>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(x => x.PersonalIdentityNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.College)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(x => x.DateOfBirth)
                .IsRequired();

            entity.Property(x => x.Status);

            entity.Property(x => x.Gender);

            entity.Property(x => x.IsAdmin);
            
            entity.Property(x => x.CreatedAt);
        });
    }
}