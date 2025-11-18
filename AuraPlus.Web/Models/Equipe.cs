namespace AuraPlus.Web.Models;

public class Equipe
{
    public int Id { get; set; }
    public string NmTime { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    // Navigation properties
    public ICollection<Users> Users { get; set; } = new List<Users>();
    public ICollection<RelatorioEquipe> RelatoriosEquipe { get; set; } = new List<RelatorioEquipe>();
}
