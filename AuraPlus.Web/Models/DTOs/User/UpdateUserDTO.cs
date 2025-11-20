using System.ComponentModel.DataAnnotations;

namespace AuraPlus.Web.Models.DTOs.User;

public class UpdateUserDTO
{
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string? Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(150, ErrorMessage = "Email deve ter no máximo 150 caracteres")]
    public string? Email { get; set; }

    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string? Password { get; set; }

    [StringLength(100, ErrorMessage = "Cargo deve ter no máximo 100 caracteres")]
    public string? Cargo { get; set; }

    public DateTime? DataAdmissao { get; set; }
}
