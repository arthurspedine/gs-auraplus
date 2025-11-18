namespace AuraPlus.Web.Models;

public class RelatorioPessoa
{
    public int Id { get; set; }
    public int NumeroIndicacoes { get; set; } = 0;
    public DateTime Data { get; set; } = DateTime.Now;
    public string? Descritivo { get; set; }
    public int IdUsuario { get; set; }

    // Navigation properties
    public Users Usuario { get; set; } = null!;
}
