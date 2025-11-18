namespace AuraPlus.Web.Models;

public class RelatorioEquipe
{
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public string? SentimentoMedio { get; set; }
    public string? Descritivo { get; set; }
    public int IdEquipe { get; set; }

    // Navigation properties
    public Equipe Equipe { get; set; } = null!;
}
