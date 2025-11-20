using System.ComponentModel.DataAnnotations;

namespace AuraPlus.Web.Models.DTOs.Equipe;

public class EntrarEquipeDTO
{
    [Required(ErrorMessage = "Cargo é obrigatório ao entrar na equipe")]
    [StringLength(100, ErrorMessage = "Cargo deve ter no máximo 100 caracteres")]
    public string Cargo { get; set; } = string.Empty;

    public DateTime? DataAdmissao { get; set; }
}
