namespace AuraPlus.Web.Models.DTOs.Equipe;

public class EquipeDTO
{
    public int Id { get; set; }
    public string NmTime { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int TotalMembros { get; set; }
    public List<MembroEquipeDTO> Membros { get; set; } = new List<MembroEquipeDTO>();

    public static EquipeDTO FromEquipe(Models.Equipe equipe)
    {
        return new EquipeDTO
        {
            Id = equipe.Id,
            NmTime = equipe.NmTime,
            Descricao = equipe.Descricao,
            TotalMembros = equipe.Users?.Count ?? 0,
            Membros = equipe.Users?.Select(u => new MembroEquipeDTO
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Cargo = u.Cargo,
                Role = u.Role,
                DataAdmissao = u.DataAdmissao,
                Ativo = u.Ativo == '1'
            }).ToList() ?? new List<MembroEquipeDTO>()
        };
    }
}

public class MembroEquipeDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime? DataAdmissao { get; set; }
    public bool Ativo { get; set; }
}
