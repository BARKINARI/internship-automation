using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class StajBasvuruConfiguration : IEntityTypeConfiguration<StajBasvuru>
{
    public void Configure(EntityTypeBuilder<StajBasvuru> builder)
    {
        builder.ToTable("StajBasvurulari").HasKey(s => s.Id);

        builder.Property(s => s.Id).HasColumnName("Id").IsRequired();
        builder.Property(s => s.Ad).HasColumnName("Ad").HasMaxLength(50).IsRequired();
        builder.Property(s => s.Soyad).HasColumnName("Soyad").HasMaxLength(50).IsRequired();
        builder.Property(s => s.Telefon).HasColumnName("Telefon").HasMaxLength(20).IsRequired();
        builder.Property(s => s.Eposta).HasColumnName("Eposta").HasMaxLength(100).IsRequired();
        builder.Property(s => s.Okul).HasColumnName("Okul").HasMaxLength(150).IsRequired();
        builder.Property(s => s.Bolum).HasColumnName("Bolum").HasMaxLength(150).IsRequired();
        builder.Property(s => s.Sinif).HasColumnName("Sinif").HasMaxLength(20).IsRequired();
        builder.Property(s => s.StajBaslangicTarihi).HasColumnName("StajBaslangicTarihi").IsRequired();
        builder.Property(s => s.StajBitisTarihi).HasColumnName("StajBitisTarihi").IsRequired();
        builder.Property(s => s.Referans).HasColumnName("Referans").HasMaxLength(250).IsRequired(false);
        builder.Property(s => s.KendiniTanitmaYazisi).HasColumnName("KendiniTanitmaYazisi").HasMaxLength(1000).IsRequired(false);
        builder.Property(s => s.CvDosyaYolu).HasColumnName("CvDosyaYolu").HasMaxLength(500).IsRequired(false);
        builder.Property(s => s.StajKonusu).HasColumnName("StajKonusu").HasMaxLength(250).IsRequired(false);
        builder.Property(s => s.StajBasvuruDurumu).HasColumnName("StajBasvuruDurumu").IsRequired();
        builder.Property(s => s.UserId).HasColumnName("UserId").IsRequired(false);
        builder.Property(s => s.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(s => s.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(s => s.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(s => !s.DeletedDate.HasValue);
    }
}