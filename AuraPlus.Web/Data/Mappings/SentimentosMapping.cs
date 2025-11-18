using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuraPlus.Web.Models;

namespace AuraPlus.Web.Data.Mappings;

public class SentimentosMapping : IEntityTypeConfiguration<Sentimentos>
{
    public void Configure(EntityTypeBuilder<Sentimentos> builder)
    {
        builder.ToTable("t_arp_sentimentos");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.NomeSentimento)
            .HasColumnName("nome_sentimento")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.ValorPontuacao)
            .HasColumnName("valor_pontuacao")
            .HasPrecision(5, 2);

        builder.Property(s => s.Data)
            .HasColumnName("data")
            .HasColumnType("TIMESTAMP")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(s => s.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(255);

        builder.Property(s => s.IdUsuario)
            .HasColumnName("id_usuario")
            .IsRequired();

        // Relationships
        builder.HasOne(s => s.Usuario)
            .WithMany(u => u.Sentimentos)
            .HasForeignKey(s => s.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
