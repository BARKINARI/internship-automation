using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class ProjeConfiguration : IEntityTypeConfiguration<Proje>
{
    public void Configure(EntityTypeBuilder<Proje> builder)
    {
        builder.ToTable("Projeler").HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("Id").IsRequired();
        builder.Property(p => p.Baslik).HasColumnName("Baslik").HasMaxLength(200).IsRequired();
        builder.Property(p => p.Aciklama).HasColumnName("Aciklama").HasMaxLength(4000).IsRequired();
        builder.Property(p => p.BaslangicTarihi).HasColumnName("BaslangicTarihi").IsRequired();
        builder.Property(p => p.BitisTarihi).HasColumnName("BitisTarihi").IsRequired();
        builder.Property(p => p.Durum).HasColumnName("Durum").IsRequired();
        builder.Property(p => p.YoneticiId).HasColumnName("YoneticiId").IsRequired(false);
        builder.Property(p => p.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(p => p.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(p => p.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(p => p.Yonetici)
            .WithMany()
            .HasForeignKey(p => p.YoneticiId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(p => !p.DeletedDate.HasValue);
    }
}
