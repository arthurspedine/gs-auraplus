using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuraPlus.Web.Models;

namespace AuraPlus.Web.Data.Mappings;

public class RelatorioPessoaMapping : IEntityTypeConfiguration<RelatorioPessoa>
{
    public void Configure(EntityTypeBuilder<RelatorioPessoa> builder)
    {
        builder.ToTable("t_arp_relatorio_pessoa");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.NumeroIndicacoes)
            .HasColumnName("numero_indicacoes")
            .HasPrecision(5, 0)
            .HasDefaultValue(0);

        builder.Property(r => r.Data)
            .HasColumnName("data")
            .HasColumnType("TIMESTAMP")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.Descritivo)
            .HasColumnName("descritivo")
            .HasMaxLength(255);

        builder.Property(r => r.IdUsuario)
            .HasColumnName("id_usuario")
            .IsRequired();

        // Relationships
        builder.HasOne(r => r.Usuario)
            .WithMany(u => u.RelatoriosPessoa)
            .HasForeignKey(r => r.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
