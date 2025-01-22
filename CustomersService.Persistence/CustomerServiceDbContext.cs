using CustomersService.Persistence.Configuration.EntityConfigurations;
using CustomersService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomersService.Persistence
{
    public class CustomerServiceDbContext(DbContextOptions<CustomerServiceDbContext> options): DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
        }

        public void BeginTransaction()
        {
            Database.BeginTransaction();
        }

        public void CommitTransaction() 
        { 
            Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            Database.RollbackTransaction();
        }
    }
}
