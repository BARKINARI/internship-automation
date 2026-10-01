using Core.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User<int>>
{
    public void Configure(EntityTypeBuilder<User<int>> builder)
    {
        builder.ToTable("Users").HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("Id").IsRequired();
        builder.Property(u => u.FirstName).HasColumnName("FirstName").HasMaxLength(50).IsRequired();
        builder.Property(u => u.LastName).HasColumnName("LastName").HasMaxLength(50).IsRequired();
        builder.Property(u => u.Email).HasColumnName("Email").HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Email, "UK_Users_Email").IsUnique();
        builder.Property(u => u.PasswordHash).HasColumnName("PasswordHash").IsRequired();
        builder.Property(u => u.PasswordSalt).HasColumnName("PasswordSalt").IsRequired();
        builder.Property(u => u.Status).HasColumnName("Status").HasDefaultValue(true).IsRequired();
        builder.Property(u => u.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(u => u.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(u => u.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(u => !u.DeletedDate.HasValue);

        byte[] passwordSalt = Convert.FromBase64String("y5OpP17E9LN9A0W6qIi32PC96ND/HiIel5tQO+n/fawTSTBvl6Jbo7FHGlAWtRHtR5SF/FFhoxE/GOhyTteOS2JwHrv2U1btcJgU7YY1GIeh0XJMWDEDXUUozmzNat7gUYjhuXfAzPDj0A7X884JYqiJrm45WElaR9UgkXeRRXc=");
        byte[] passwordHash = Convert.FromBase64String("GTiZ8XGu980E5d+lvq3Z3M0QW3JmqFCJhdhYic9Mnjx7uPIs5pe6NUCAJBW4nLOBeKt6B5QWDyR+WJx7Tl0PBg==");

        User<int> adminUser = new(
            id: 1,
            firstName: "Sistem",
            lastName: "Yöneticisi",
            email: "admin@ae.com",
            passwordSalt: passwordSalt,
            passwordHash: passwordHash,
            status: true
        )
        {
            CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        builder.HasData(adminUser);
    }
}