using Linnked.Core.Domain.Entities;
using Linnked;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Linnked.Infrastructure
{

    public class AppDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<WaitList> WaitLists { get; set; }
        public DbSet<ScrambledMessageMapping> ScrambledMessageMappings { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<Role> Roles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<ScrambledMessageMapping>()
                .HasKey(smm => smm.Id);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                    }
                }
            }

            modelBuilder.Entity<Role>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<User>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<VerificationCode>().Property<int>("Id").ValueGeneratedOnAdd();

            modelBuilder.Entity<VerificationCode>()
                .HasOne(vc => vc.User)
                .WithMany(u => u.VerificationCodes)
                .HasForeignKey(vc => vc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, DateCreated = DateTime.UtcNow, Name = "Admin", CreatedBy = "1" },
                new Role { Id = 2, DateCreated = DateTime.UtcNow, Name = "Patient", CreatedBy = "1" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    DateCreated = DateTime.UtcNow,
                    FirstName = "Linnked",
                    LastName = "Linnked",
                    IsDeleted = false,
                    Email = "hellolinnked@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("SeyiHasbiySegun"),
                    RoleId = 1,
                    CreatedBy = "Admin",
                });
        }



    }
}