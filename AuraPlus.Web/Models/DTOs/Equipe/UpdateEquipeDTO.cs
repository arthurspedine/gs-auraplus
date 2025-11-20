using System.ComponentModel.DataAnnotations;

namespace AuraPlus.Web.Models.DTOs.Equipe;

public class UpdateEquipeDTO
{
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string? NmTime { get; set; }

    [StringLength(255, ErrorMessage = "Descrição deve ter no máximo 255 caracteres")]
    public string? Descricao { get; set; }
}
