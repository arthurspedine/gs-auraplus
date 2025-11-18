using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuraPlus.Web.Models;

namespace AuraPlus.Web.Data.Mappings;

public class RelatorioEquipeMapping : IEntityTypeConfiguration<RelatorioEquipe>
{
    public void Configure(EntityTypeBuilder<RelatorioEquipe> builder)
    {
        builder.ToTable("t_arp_relatorio_equipe");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Data)
            .HasColumnName("data")
            .HasColumnType("TIMESTAMP")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.SentimentoMedio)
            .HasColumnName("sentimento_medio")
            .HasMaxLength(100);

        builder.Property(r => r.Descritivo)
            .HasColumnName("descritivo")
            .HasMaxLength(255);

        builder.Property(r => r.IdEquipe)
            .HasColumnName("id_equipe")
            .IsRequired();

        // Relationships
        builder.HasOne(r => r.Equipe)
            .WithMany(e => e.RelatoriosEquipe)
            .HasForeignKey(r => r.IdEquipe)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
