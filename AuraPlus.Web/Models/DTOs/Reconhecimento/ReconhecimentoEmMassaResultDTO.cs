namespace AuraPlus.Web.Models.DTOs.Reconhecimento;

/// <summary>
/// DTO de resposta para reconhecimento em massa
/// </summary>
public class ReconhecimentoEmMassaResultDTO
{
    public int Sucessos { get; set; }
    public int Falhas { get; set; }
    public List<ReconhecimentoDetalheDTO> Detalhes { get; set; } = new();
}

public class ReconhecimentoDetalheDTO
{
    public int IdReceptor { get; set; }
    public string Status { get; set; } = string.Empty; // "sucesso" ou "falha"
    public int? Id { get; set; } // ID do reconhecimento criado (se sucesso)
    public string? Erro { get; set; } // Mensagem de erro (se falha)
}
