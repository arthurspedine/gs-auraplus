namespace AuraPlus.Web.Models.DTOs.Auth;

public class UserInfoDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public DateTime? DataAdmissao { get; set; }
    public bool Ativo { get; set; }
    public int? IdEquipe { get; set; }
    public string? NomeEquipe { get; set; }

    public static UserInfoDTO FromUser(Users user)
    {
        return new UserInfoDTO
        {
            Id = user.Id,
            Nome = user.Nome,
            Email = user.Email,
            Role = user.Role,
            Cargo = user.Cargo,
            DataAdmissao = user.DataAdmissao,
            Ativo = user.Ativo == '1',
            IdEquipe = user.IdEquipe,
            NomeEquipe = user.Equipe?.NmTime
        };
    }
}
