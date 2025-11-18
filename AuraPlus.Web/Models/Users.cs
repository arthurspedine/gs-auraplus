namespace AuraPlus.Web.Models;

public class Users
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public DateTime? DataAdmissao { get; set; }
    public char Ativo { get; set; } = '1';
    public int? IdEquipe { get; set; }

    // Navigation properties
    public Equipe? Equipe { get; set; }
    public ICollection<Sentimentos> Sentimentos { get; set; } = new List<Sentimentos>();
    public ICollection<Reconhecimento> ReconhecimentosRecebidos { get; set; } = new List<Reconhecimento>();
    public ICollection<Reconhecimento> ReconhecimentosFeitos { get; set; } = new List<Reconhecimento>();
    public ICollection<RelatorioPessoa> RelatoriosPessoa { get; set; } = new List<RelatorioPessoa>();
}
