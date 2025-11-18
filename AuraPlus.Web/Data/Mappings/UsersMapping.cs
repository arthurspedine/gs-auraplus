using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuraPlus.Web.Models;

namespace AuraPlus.Web.Data.Mappings;

public class UsersMapping : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.ToTable("t_arp_users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Nome)
            .HasColumnName("nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Senha)
            .HasColumnName("senha")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Cargo)
            .HasColumnName("cargo")
            .HasMaxLength(100);

        builder.Property(u => u.DataAdmissao)
            .HasColumnName("data_admissao")
            .HasColumnType("TIMESTAMP");

        builder.Property(u => u.Ativo)
            .HasColumnName("ativo")
            .HasMaxLength(1)
            .HasDefaultValue('1')
            .IsRequired();

        builder.Property(u => u.IdEquipe)
            .HasColumnName("id_equipe");

        // Relationships
        builder.HasOne(u => u.Equipe)
            .WithMany(e => e.Users)
            .HasForeignKey(u => u.IdEquipe)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.Sentimentos)
            .WithOne(s => s.Usuario)
            .HasForeignKey(s => s.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ReconhecimentosRecebidos)
            .WithOne(r => r.Reconhecido)
            .HasForeignKey(r => r.IdReconhecido)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ReconhecimentosFeitos)
            .WithOne(r => r.Reconhecedor)
            .HasForeignKey(r => r.IdReconhecedor)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RelatoriosPessoa)
            .WithOne(r => r.Usuario)
            .HasForeignKey(r => r.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
