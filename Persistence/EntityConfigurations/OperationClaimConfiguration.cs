using Application.Features.OperationClaims.Constants;
using Application.Features.StajBasvurulari.Constants;
using Application.Features.UserOperationClaims.Constants;
using Application.Features.Users.Constants;
using Core.Security.Constants;
using Core.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim<int>>
{
    public void Configure(EntityTypeBuilder<OperationClaim<int>> builder)
    {
        builder.ToTable("OperationClaims").HasKey(o => o.Id);

        builder.Property(o => o.Id).HasColumnName("Id").IsRequired();
        builder.Property(o => o.Name).HasColumnName("Name").HasMaxLength(100).IsRequired();
        builder.HasIndex(o => o.Name, "UK_OperationClaims_Name").IsUnique();
        builder.Property(o => o.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(o => o.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(o => o.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(o => !o.DeletedDate.HasValue);

        OperationClaim<int>[] seeds =
        [
            // Genel Super Admin
            new(id: 1, name: GeneralOperationClaims.Admin),

            new(id: 2, name: StajBasvuruOperationClaims.Read),
            new(id: 3, name: StajBasvuruOperationClaims.Write),
            new(id: 4, name: StajBasvuruOperationClaims.Add),
            new(id: 5, name: StajBasvuruOperationClaims.Update),
            new(id: 6, name: StajBasvuruOperationClaims.Delete),

            new(id: 7, name: UsersOperationClaims.Read),
            new(id: 8, name: UsersOperationClaims.Write),
            new(id: 9, name: UsersOperationClaims.Add),
            new(id: 10, name: UsersOperationClaims.Update),
            new(id: 11, name: UsersOperationClaims.Delete),

            new(id: 12, name: OperationClaimsOperationClaims.Read),
            new(id: 13, name: OperationClaimsOperationClaims.Write),
            new(id: 14, name: OperationClaimsOperationClaims.Add),
            new(id: 15, name: OperationClaimsOperationClaims.Update),
            new(id: 16, name: OperationClaimsOperationClaims.Delete),

            new(id: 17, name: UserOperationClaimsOperationClaims.Read),
            new(id: 18, name: UserOperationClaimsOperationClaims.Write),
            new(id: 19, name: UserOperationClaimsOperationClaims.Add),
            new(id: 20, name: UserOperationClaimsOperationClaims.Update),
            new(id: 21, name: UserOperationClaimsOperationClaims.Delete),

            // Rol Tanimlari
            new(id: 22, name: "ProjeYoneticisi"),
            new(id: 23, name: "Stajyer"),
            new(id: 24, name: "StajSorumlusu"),
            new(id: 25, name: "Egitmen"),

            // Moduler Menu Yetkileri (Sidebar modulleri ile birebir eslesen)
            new(id: 26, name: ModularOperationClaims.StajBasvurular),
            new(id: 27, name: ModularOperationClaims.StajYeniBasvuru),
            new(id: 28, name: ModularOperationClaims.ProjeYonetimi),
            new(id: 29, name: ModularOperationClaims.ProjeTakipNotlandirma),
            new(id: 30, name: ModularOperationClaims.IletisimOgrenciMesajlari)
        ];
        builder.HasData(seeds);
    }
}
