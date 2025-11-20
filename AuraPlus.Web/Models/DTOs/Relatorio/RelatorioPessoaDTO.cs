namespace AuraPlus.Web.Models.DTOs.Relatorio;

public class RelatorioPessoaDTO
{
    public int Id { get; set; }
    public int NumeroIndicacoes { get; set; }
    public DateTime Data { get; set; }
    public string? Descritivo { get; set; }
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string? EmailUsuario { get; set; }
    public string? CargoUsuario { get; set; }

    public static RelatorioPessoaDTO FromRelatorioPessoa(Models.RelatorioPessoa relatorio)
    {
        return new RelatorioPessoaDTO
        {
            Id = relatorio.Id,
            NumeroIndicacoes = relatorio.NumeroIndicacoes,
            Data = relatorio.Data,
            Descritivo = relatorio.Descritivo,
            IdUsuario = relatorio.IdUsuario,
            NomeUsuario = relatorio.Usuario?.Nome ?? string.Empty,
            EmailUsuario = relatorio.Usuario?.Email,
            CargoUsuario = relatorio.Usuario?.Cargo
        };
    }
}
