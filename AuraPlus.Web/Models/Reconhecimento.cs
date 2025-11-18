namespace AuraPlus.Web.Models;

public class Reconhecimento
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public int IdReconhecedor { get; set; }
    public int IdReconhecido { get; set; }

    // Navigation properties
    public Users Reconhecedor { get; set; } = null!;
    public Users Reconhecido { get; set; } = null!;
}
