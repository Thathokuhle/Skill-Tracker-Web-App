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
            //This is the connection String - Read from the environment variable.
            //optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionString"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        //entities
        public DbSet<Account.GetUserByEmail> AccountUserByEmail { get; set; }
        public DbSet<Account.CreateAccount> CreateAccount { get; set; }


        //Admin
        public DbSet<Admin.GetAllUsers> GetAllUsers { get; set; }

        public DbSet<Account.UserRole> UserRoles { get; set; }

    }
}