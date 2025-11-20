using AuraPlus.Web.Models.DTOs.Relatorio;

namespace AuraPlus.Web.Services;

public interface IRelatorioService
{
    Task<RelatorioPessoaDTO> GerarRelatorioPessoaAsync(int usuarioId);
    Task<RelatorioEquipeDTO> GerarRelatorioEquipeAsync(int equipeId);
    Task<RelatorioPessoaDTO?> GetRelatorioPessoaByIdAsync(int id);
    Task<RelatorioEquipeDTO?> GetRelatorioEquipeByIdAsync(int id);
    Task<IEnumerable<RelatorioPessoaDTO>> GetRelatoriosPessoaUsuarioAsync(int usuarioId);
    Task<IEnumerable<RelatorioEquipeDTO>> GetRelatoriosEquipeAsync(int equipeId);
}
