using System.ComponentModel.DataAnnotations;

namespace AuraPlus.Web.Models.DTOs.Equipe;

public class CreateEquipeDTO
{
    [Required(ErrorMessage = "Nome da equipe é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string NmTime { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Descrição deve ter no máximo 255 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Cargo é obrigatório ao criar equipe")]
    [StringLength(100, ErrorMessage = "Cargo deve ter no máximo 100 caracteres")]
    public string Cargo { get; set; } = string.Empty;

    public DateTime? DataAdmissao { get; set; }
}
