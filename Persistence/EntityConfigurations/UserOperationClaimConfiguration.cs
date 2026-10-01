using Core.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class UserOperationClaimConfiguration : IEntityTypeConfiguration<UserOperationClaim<int, int, int>>
{
    public void Configure(EntityTypeBuilder<UserOperationClaim<int, int, int>> builder)
    {
        builder.ToTable("UserOperationClaims").HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("Id").IsRequired();
        builder.Property(u => u.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(u => u.OperationClaimId).HasColumnName("OperationClaimId").IsRequired();
        builder.Property(u => u.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(u => u.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(u => u.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(u => !u.DeletedDate.HasValue);

        UserOperationClaim<int, int, int> adminClaim = new(
            id: 1,
            userId: 1,
            operationClaimId: 1
        )
        {
            CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        builder.HasData(adminClaim);
    }
}