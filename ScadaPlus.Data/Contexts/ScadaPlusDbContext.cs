using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScadaPlus.Data.Identity;
using ScadaPlus.Data.Models;

namespace ScadaPlus.Data.Contexts;

public class ScadaPlusDbContext : IdentityDbContext
{
    public DbSet<Machine> Machines { get; set; }
    public DbSet<Job> Jobs { get; set; }

    public ScadaPlusDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Machine>()
            .HasData(
            new Machine
            {
                MachineId = 1,
                Code = "CNC-001"
            },
            new Machine
            {
                MachineId = 2,
                Code = "CNC-002"
            },
            new Machine
            {
                MachineId = 3,
                Code = "CNC-003"
            });

        var adminRoleId = Guid.NewGuid().ToString();
        var operatorRoleId = Guid.NewGuid().ToString();
        var adminUserId = Guid.NewGuid().ToString();
        var operatorUserId = Guid.NewGuid().ToString();

        builder.Entity<IdentityRole>()
            .HasData(
            new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin"
            },
            new IdentityRole
            {
                Id = operatorRoleId,
                Name = "Operator"
            });

        builder.Entity<ScadaPlusIdentityUser>()
            .HasData(
            new ScadaPlusIdentityUser
            {
                Id = adminUserId,
                UserName = "AdminScadaPlus",
                NormalizedUserName = "ADMINSCADAPLUS",
                PasswordHash = HashPassword("AdminScadaPlus"),
                Name = "Admin SP"
            },
            new ScadaPlusIdentityUser
            {
                Id = operatorUserId,
                UserName = "OperatorScadaPlus",
                NormalizedUserName = "OPERATORSCADAPLUS",
                PasswordHash = HashPassword("OperatorScadaPlus"),
                Name = "Operator SP"
            });

        builder.Entity<IdentityUserRole<string>>()
            .HasData(
            new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            },
            new IdentityUserRole<string>
            {
                RoleId = operatorRoleId,
                UserId = operatorUserId
            });
    }

    private static string HashPassword(string password)
    {
        var hasher = new PasswordHasher<ScadaPlusIdentityUser>();
        return hasher.HashPassword(null, password);
    }
}
