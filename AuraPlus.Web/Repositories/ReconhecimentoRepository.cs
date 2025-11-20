using AuraPlus.Web.Data;
using AuraPlus.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AuraPlus.Web.Repositories;

public class ReconhecimentoRepository : IReconhecimentoRepository
{
    private readonly OracleDbContext _context;

    public ReconhecimentoRepository(OracleDbContext context)
    {
        _context = context;
    }

    public async Task<Reconhecimento> AddAsync(Reconhecimento reconhecimento)
    {
        _context.Reconhecimentos.Add(reconhecimento);
        await _context.SaveChangesAsync();
        return reconhecimento;
    }

    public async Task<Reconhecimento?> GetByIdAsync(int id)
    {
        return await _context.Reconhecimentos
            .Include(r => r.Reconhecedor)
            .Include(r => r.Reconhecido)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Reconhecimento>> GetByReconhecedorIdAsync(int reconhecedorId)
    {
        return await _context.Reconhecimentos
            .Include(r => r.Reconhecido)
            .Where(r => r.IdReconhecedor == reconhecedorId)
            .OrderByDescending(r => r.Data)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reconhecimento>> GetByReconhecidoIdAsync(int reconhecidoId)
    {
        return await _context.Reconhecimentos
            .Include(r => r.Reconhecedor)
            .Where(r => r.IdReconhecido == reconhecidoId)
            .OrderByDescending(r => r.Data)
            .ToListAsync();
    }

    public async Task<bool> JaReconheceuHojeAsync(int reconhecedorId)
    {
        var hoje = DateTime.Today;
        var amanha = hoje.AddDays(1);
        
        var count = await _context.Reconhecimentos
            .Where(r => r.IdReconhecedor == reconhecedorId && r.Data >= hoje && r.Data < amanha)
            .CountAsync();
            
        return count > 0;
    }

    public async Task<bool> JaReconheceuPessoaNoMesAsync(int reconhecedorId, int reconhecidoId, DateTime mes)
    {
        var inicioMes = new DateTime(mes.Year, mes.Month, 1);
        var fimMes = inicioMes.AddMonths(1);
        
        var count = await _context.Reconhecimentos
            .Where(r => r.IdReconhecedor == reconhecedorId 
                && r.IdReconhecido == reconhecidoId
                && r.Data >= inicioMes 
                && r.Data < fimMes)
            .CountAsync();
            
        return count > 0;
    }

    public async Task<int> GetTotalReconhecimentosRecebidosAsync(int usuarioId, DateTime dataInicio, DateTime dataFim)
    {
        return await _context.Reconhecimentos
            .Where(r => r.IdReconhecido == usuarioId && r.Data >= dataInicio && r.Data < dataFim)
            .CountAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var reconhecimento = await _context.Reconhecimentos.FindAsync(id);
        if (reconhecimento == null)
            return false;

        _context.Reconhecimentos.Remove(reconhecimento);
        await _context.SaveChangesAsync();
        return true;
    }
}
