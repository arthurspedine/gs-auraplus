using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuraPlus.Web.Models;

namespace AuraPlus.Web.Data.Mappings;

public class ReconhecimentoMapping : IEntityTypeConfiguration<Reconhecimento>
{
    public void Configure(EntityTypeBuilder<Reconhecimento> builder)
    {
        builder.ToTable("t_arp_reconhecimento");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Titulo)
            .HasColumnName("titulo")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(255);

        builder.Property(r => r.Data)
            .HasColumnName("data")
            .HasColumnType("TIMESTAMP")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.IdReconhecedor)
            .HasColumnName("id_reconhecedor")
            .IsRequired();

        builder.Property(r => r.IdReconhecido)
            .HasColumnName("id_reconhecido")
            .IsRequired();

        // Relationships
        builder.HasOne(r => r.Reconhecedor)
            .WithMany(u => u.ReconhecimentosFeitos)
            .HasForeignKey(r => r.IdReconhecedor)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Reconhecido)
            .WithMany(u => u.ReconhecimentosRecebidos)
            .HasForeignKey(r => r.IdReconhecido)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
