namespace AuraPlus.Web.Models;

public class Sentimentos
{
    public int Id { get; set; }
    public string NomeSentimento { get; set; } = string.Empty;
    public decimal? ValorPontuacao { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public string? Descricao { get; set; }
    public int IdUsuario { get; set; }

    // Navigation properties
    public Users Usuario { get; set; } = null!;
}
