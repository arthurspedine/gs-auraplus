namespace AuraPlus.Web.Models.DTOs.Relatorio;

public class RelatorioEquipeDTO
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public string? SentimentoMedio { get; set; }
    public string? Descritivo { get; set; }
    public int IdEquipe { get; set; }
    public string NomeEquipe { get; set; } = string.Empty;
    public int TotalMembros { get; set; }

    public static RelatorioEquipeDTO FromRelatorioEquipe(Models.RelatorioEquipe relatorio)
    {
        return new RelatorioEquipeDTO
        {
            Id = relatorio.Id,
            Data = relatorio.Data,
            SentimentoMedio = relatorio.SentimentoMedio,
            Descritivo = relatorio.Descritivo,
            IdEquipe = relatorio.IdEquipe,
            NomeEquipe = relatorio.Equipe?.NmTime ?? string.Empty,
            TotalMembros = relatorio.Equipe?.Users?.Count(u => u.Ativo == '1') ?? 0
        };
    }
}
