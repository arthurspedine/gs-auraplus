using System.ComponentModel.DataAnnotations;

namespace AuraPlus.Web.Models.DTOs.Reconhecimento;

public class CreateReconhecimentoDTO
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [StringLength(100, ErrorMessage = "Título deve ter no máximo 100 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "ID do reconhecido é obrigatório")]
    public int IdReconhecido { get; set; }
}
