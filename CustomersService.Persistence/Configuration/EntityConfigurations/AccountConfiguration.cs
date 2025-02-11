using CustomersService.Persistence.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CustomersService.Persistence.Configuration.EntityConfigurations;

internal class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
        .IsRequired()
        .ValueGeneratedOnAdd();

        builder.Property(p => p.DateCreated)
        .IsRequired()
        .HasColumnType("timestamp without time zone")
        .HasDefaultValueSql("NOW()");

        builder.Property(p => p.Currency)
       .IsRequired();

        builder.Property(p => p.IsDeactivated)
        .IsRequired()
        .HasDefaultValue(false);

        builder.HasOne(p => p.Customer)
       .WithMany(u => u.Accounts)
       .HasForeignKey(c => c.CustomerId)
       .OnDelete(DeleteBehavior.Restrict);
    }
}
