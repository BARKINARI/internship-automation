using Core.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken<int, int>>
{
    public void Configure(EntityTypeBuilder<RefreshToken<int, int>> builder)
    {
        builder.ToTable("RefreshTokens").HasKey(r => r.Id);

        builder.Property(r => r.Id).HasColumnName("Id").IsRequired();
        builder.Property(r => r.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(r => r.Token).HasColumnName("Token").HasMaxLength(500).IsRequired();
        builder.Property(r => r.ExpirationDate).HasColumnName("ExpirationDate").IsRequired();
        builder.Property(r => r.CreatedByIp).HasColumnName("CreatedByIp").HasMaxLength(50).IsRequired();
        builder.Property(r => r.RevokedDate).HasColumnName("RevokedDate");
        builder.Property(r => r.RevokedByIp).HasColumnName("RevokedByIp").HasMaxLength(50);
        builder.Property(r => r.ReplacedByToken).HasColumnName("ReplacedByToken").HasMaxLength(500);
        builder.Property(r => r.ReasonRevoked).HasColumnName("ReasonRevoked").HasMaxLength(250);
        builder.Property(r => r.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(r => r.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(r => r.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(r => !r.DeletedDate.HasValue);
    }
}