using DataLogic.Admin;
using Microsoft.EntityFrameworkCore;

namespace DataLogic
{
    public class DefaultContext : DbContext
    {
        public DefaultContext()
        {

        }
        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GetAllUsers>(entity =>
            {
                entity.ToTable("User");
                entity.Property(u => u.Email).HasColumnName("EmailAddress");
            });

            base.OnModelCreating(modelBuilder);
        }

        //entities
        public DbSet<Account.GetUserByEmail> AccountUserByEmail { get; set; }
        public DbSet<Account.CreateAccount> CreateAccount { get; set; }


        //Admin
        public DbSet<Admin.GetAllUsers> GetAllUsers { get; set; }

        public DbSet<Account.UserRole> UserRoles { get; set; }

    }
}