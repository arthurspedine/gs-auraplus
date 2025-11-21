using System.ComponentModel.DataAnnotations;

namespace AuraPlus.Web.Models.DTOs.Reconhecimento;

/// <summary>
/// DTO para criação de múltiplos reconhecimentos de uma vez
/// Disponível apenas na API v2
/// </summary>
public class CreateReconhecimentoEmMassaDTO
{
    [Required(ErrorMessage = "Lista de reconhecimentos é obrigatória")]
    [MinLength(1, ErrorMessage = "Deve haver pelo menos 1 reconhecimento")]
    [MaxLength(10, ErrorMessage = "Máximo de 10 reconhecimentos por requisição")]
    public List<ReconhecimentoItemDTO> Reconhecimentos { get; set; } = new();
}

public class ReconhecimentoItemDTO
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [StringLength(100, ErrorMessage = "Título deve ter no máximo 100 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "ID do receptor é obrigatório")]
    public int IdReceptor { get; set; }
}
