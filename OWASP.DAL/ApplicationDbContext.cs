using Microsoft.EntityFrameworkCore;
using OWASP.DAL.Infrastructure;
using OWASP.DAL.Models;
using OWASP.DAL.Security;
using System.Linq;

namespace OWASP.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WeakPassword> WeakPasswords { get; set; }

        private SecurityService _securityService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, SecurityService securityService) 
            : base(options)
        {
            _securityService = securityService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeakPassword>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Password).IsUnique();
                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsRequired()
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.FirstName).HasMaxLength(255);
                entity.Property(e => e.LastName).HasMaxLength(255);
                entity.Property(e => e.EmailAddress).HasMaxLength(255);
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasValueGenerator<SaltGenerator>();
            });
        }

        public override int SaveChanges()
        {
            var newUsers = ChangeTracker.Entries<User>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);

            foreach(var user in newUsers)
            {
                user.Password = _securityService.ProtectPassword(user);
            }

            return base.SaveChanges();
        }
    }
}
