namespace AuraPlus.Web.Models.DTOs.Reconhecimento;

public class ReconhecimentoDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime Data { get; set; }
    public int IdReconhecedor { get; set; }
    public string NomeReconhecedor { get; set; } = string.Empty;
    public int IdReconhecido { get; set; }
    public string NomeReconhecido { get; set; } = string.Empty;

    public static ReconhecimentoDTO FromReconhecimento(Models.Reconhecimento reconhecimento)
    {
        return new ReconhecimentoDTO
        {
            Id = reconhecimento.Id,
            Titulo = reconhecimento.Titulo,
            Descricao = reconhecimento.Descricao,
            Data = reconhecimento.Data,
            IdReconhecedor = reconhecimento.IdReconhecedor,
            NomeReconhecedor = reconhecimento.Reconhecedor?.Nome ?? string.Empty,
            IdReconhecido = reconhecimento.IdReconhecido,
            NomeReconhecido = reconhecimento.Reconhecido?.Nome ?? string.Empty
        };
    }
}
