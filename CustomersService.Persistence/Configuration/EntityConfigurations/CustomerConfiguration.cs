using CustomersService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomersService.Persistence.Configuration.EntityConfigurations;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
        .IsRequired()
        .ValueGeneratedOnAdd();

        builder.Property(p => p.Role)
        .IsRequired();

        builder.Property(p => p.FirstName)
        .IsRequired()
        .HasMaxLength(20);

        builder.Property(p => p.LastName)
        .IsRequired()
        .HasMaxLength(20);

        builder.Property(p => p.Email)
        .IsRequired()
        .HasMaxLength(50);

        builder.Property(p => p.Password)
        .IsRequired()
        .HasMaxLength(15);

        builder.Property(p => p.Address)
        .IsRequired()
        .HasMaxLength(150);

        builder.Property(p => p.Phone)
        .IsRequired()
        .HasMaxLength(15);

        builder.Property(p => p.BirthDate)
        .IsRequired();

        builder.Property(p => p.CustomVipDueDate)
       .IsRequired(false)
       .HasColumnType("timestamp without time zone");

        builder.Property(p => p.IsDeactivated)
        .IsRequired()
        .HasDefaultValue(false);
    }
}
