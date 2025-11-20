using AuraPlus.Web.Models;

namespace AuraPlus.Web.Repositories;

public interface IRelatorioRepository
{
    Task<RelatorioPessoa> AddRelatorioPessoaAsync(RelatorioPessoa relatorio);
    Task<RelatorioEquipe> AddRelatorioEquipeAsync(RelatorioEquipe relatorio);
    Task<RelatorioPessoa?> GetRelatorioPessoaByIdAsync(int id);
    Task<RelatorioEquipe?> GetRelatorioEquipeByIdAsync(int id);
    Task<IEnumerable<RelatorioPessoa>> GetRelatoriosPessoaPorUsuarioAsync(int usuarioId);
    Task<IEnumerable<RelatorioEquipe>> GetRelatoriosEquipePorEquipeAsync(int equipeId);
    Task<RelatorioPessoa?> GetUltimoRelatorioPessoaAsync(int usuarioId);
    Task<RelatorioEquipe?> GetUltimoRelatorioEquipeAsync(int equipeId);
}
