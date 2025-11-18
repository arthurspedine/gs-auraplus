using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuraPlus.Web.Models;

namespace AuraPlus.Web.Data.Mappings;

public class EquipeMapping : IEntityTypeConfiguration<Equipe>
{
    public void Configure(EntityTypeBuilder<Equipe> builder)
    {
        builder.ToTable("t_arp_equipe");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.NmTime)
            .HasColumnName("nm_time")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(255);

        // Relationships
        builder.HasMany(e => e.Users)
            .WithOne(u => u.Equipe)
            .HasForeignKey(u => u.IdEquipe)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.RelatoriosEquipe)
            .WithOne(r => r.Equipe)
            .HasForeignKey(r => r.IdEquipe)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
