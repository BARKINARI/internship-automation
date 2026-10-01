using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class MesajConfiguration : IEntityTypeConfiguration<Mesaj>
{
    public void Configure(EntityTypeBuilder<Mesaj> builder)
    {
        builder.ToTable("Mesajlar").HasKey(m => m.Id);

        builder.Property(m => m.Id).HasColumnName("Id").IsRequired();
        builder.Property(m => m.GonderenId).HasColumnName("GonderenId").IsRequired();
        builder.Property(m => m.AliciId).HasColumnName("AliciId").IsRequired();
        builder.Property(m => m.StajBasvuruId).HasColumnName("StajBasvuruId").IsRequired(false);
        builder.Property(m => m.ProjeId).HasColumnName("ProjeId").IsRequired(false);
        builder.Property(m => m.Icerik).HasColumnName("Icerik").HasMaxLength(4000).IsRequired(false);
        builder.Property(m => m.DosyaYolu).HasColumnName("DosyaYolu").HasMaxLength(1000).IsRequired(false);
        builder.Property(m => m.DosyaAdi).HasColumnName("DosyaAdi").HasMaxLength(500).IsRequired(false);
        builder.Property(m => m.OkunduMu).HasColumnName("OkunduMu").IsRequired().HasDefaultValue(false);
        builder.Property(m => m.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(m => m.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(m => m.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(m => m.GonderenUser)
            .WithMany()
            .HasForeignKey(m => m.GonderenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.AliciUser)
            .WithMany()
            .HasForeignKey(m => m.AliciId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.StajBasvuru)
            .WithMany(s => s.Mesajlar)
            .HasForeignKey(m => m.StajBasvuruId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Proje)
            .WithMany(p => p.Mesajlar)
            .HasForeignKey(m => m.ProjeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(m => !m.DeletedDate.HasValue);
    }
}
