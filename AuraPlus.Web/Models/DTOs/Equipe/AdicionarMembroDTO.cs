using System.ComponentModel.DataAnnotations;

namespace AuraPlus.Web.Models.DTOs.Equipe;

public class AdicionarMembroDTO
{
    [Required(ErrorMessage = "ID do membro é obrigatório")]
    public int MembroId { get; set; }

    [Required(ErrorMessage = "Cargo é obrigatório")]
    [StringLength(100, ErrorMessage = "Cargo deve ter no máximo 100 caracteres")]
    public string Cargo { get; set; } = string.Empty;

    public DateTime? DataAdmissao { get; set; }
}
