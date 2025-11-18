using Microsoft.EntityFrameworkCore;
using AuraPlus.Web.Models;

namespace AuraPlus.Web.Data 
{
    public class OracleDbContext : DbContext
    {
        public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Sentimentos> Sentimentos { get; set; }
        public DbSet<Reconhecimento> Reconhecimentos { get; set; }
        public DbSet<RelatorioPessoa> RelatoriosPessoa { get; set; }
        public DbSet<RelatorioEquipe> RelatoriosEquipe { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OracleDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}