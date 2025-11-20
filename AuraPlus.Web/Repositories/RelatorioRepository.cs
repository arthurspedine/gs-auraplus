using AuraPlus.Web.Data;
using AuraPlus.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AuraPlus.Web.Repositories;

public class RelatorioRepository : IRelatorioRepository
{
    private readonly OracleDbContext _context;

    public RelatorioRepository(OracleDbContext context)
    {
        _context = context;
    }

    public async Task<RelatorioPessoa> AddRelatorioPessoaAsync(RelatorioPessoa relatorio)
    {
        _context.RelatoriosPessoa.Add(relatorio);
        await _context.SaveChangesAsync();
        return relatorio;
    }

    public async Task<RelatorioEquipe> AddRelatorioEquipeAsync(RelatorioEquipe relatorio)
    {
        _context.RelatoriosEquipe.Add(relatorio);
        await _context.SaveChangesAsync();
        return relatorio;
    }

    public async Task<RelatorioPessoa?> GetRelatorioPessoaByIdAsync(int id)
    {
        return await _context.RelatoriosPessoa
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RelatorioEquipe?> GetRelatorioEquipeByIdAsync(int id)
    {
        return await _context.RelatoriosEquipe
            .Include(r => r.Equipe)
                .ThenInclude(e => e.Users.Where(u => u.Ativo == '1'))
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<RelatorioPessoa>> GetRelatoriosPessoaPorUsuarioAsync(int usuarioId)
    {
        return await _context.RelatoriosPessoa
            .Include(r => r.Usuario)
            .Where(r => r.IdUsuario == usuarioId)
            .OrderByDescending(r => r.Data)
            .ToListAsync();
    }

    public async Task<IEnumerable<RelatorioEquipe>> GetRelatoriosEquipePorEquipeAsync(int equipeId)
    {
        return await _context.RelatoriosEquipe
            .Include(r => r.Equipe)
                .ThenInclude(e => e.Users.Where(u => u.Ativo == '1'))
            .Where(r => r.IdEquipe == equipeId)
            .OrderByDescending(r => r.Data)
            .ToListAsync();
    }

    public async Task<RelatorioPessoa?> GetUltimoRelatorioPessoaAsync(int usuarioId)
    {
        return await _context.RelatoriosPessoa
            .Include(r => r.Usuario)
            .Where(r => r.IdUsuario == usuarioId)
            .OrderByDescending(r => r.Data)
            .FirstOrDefaultAsync();
    }

    public async Task<RelatorioEquipe?> GetUltimoRelatorioEquipeAsync(int equipeId)
    {
        return await _context.RelatoriosEquipe
            .Include(r => r.Equipe)
                .ThenInclude(e => e.Users.Where(u => u.Ativo == '1'))
            .Where(r => r.IdEquipe == equipeId)
            .OrderByDescending(r => r.Data)
            .FirstOrDefaultAsync();
    }
}
