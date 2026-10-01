using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class StajyerProjeConfiguration : IEntityTypeConfiguration<StajyerProje>
{
    public void Configure(EntityTypeBuilder<StajyerProje> builder)
    {
        builder.ToTable("StajyerProjeler").HasKey(sp => sp.Id);

        builder.Property(sp => sp.Id).HasColumnName("Id").IsRequired();
        builder.Property(sp => sp.ProjeId).HasColumnName("ProjeId").IsRequired();
        builder.Property(sp => sp.StajBasvuruId).HasColumnName("StajBasvuruId").IsRequired();
        builder.Property(sp => sp.UserId).HasColumnName("UserId").IsRequired(false);
        builder.Property(sp => sp.AtanmaTarihi).HasColumnName("AtanmaTarihi").IsRequired();
        builder.Property(sp => sp.TeslimTarihi).HasColumnName("TeslimTarihi");
        builder.Property(sp => sp.Durum).HasColumnName("Durum").IsRequired();
        builder.Property(sp => sp.TeslimAciklamasi).HasColumnName("TeslimAciklamasi").HasMaxLength(2000);
        builder.Property(sp => sp.DosyaYolu).HasColumnName("DosyaYolu").HasMaxLength(500);
        builder.Property(sp => sp.Puan).HasColumnName("Puan");
        builder.Property(sp => sp.YoneticiNotu).HasColumnName("YoneticiNotu").HasMaxLength(2000);
        builder.Property(sp => sp.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(sp => sp.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(sp => sp.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(sp => sp.Proje)
            .WithMany(p => p.StajyerProjeler)
            .HasForeignKey(sp => sp.ProjeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.StajBasvuru)
            .WithMany(s => s.StajyerProjeler)
            .HasForeignKey(sp => sp.StajBasvuruId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.User)
            .WithMany()
            .HasForeignKey(sp => sp.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(sp => !sp.DeletedDate.HasValue);
    }
}
