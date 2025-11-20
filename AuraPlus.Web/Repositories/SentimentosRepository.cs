using AuraPlus.Web.Data;
using AuraPlus.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AuraPlus.Web.Repositories;

public class SentimentosRepository : ISentimentosRepository
{
    private readonly OracleDbContext _context;

    public SentimentosRepository(OracleDbContext context)
    {
        _context = context;
    }

    public async Task<Sentimentos> AddAsync(Sentimentos sentimento)
    {
        _context.Sentimentos.Add(sentimento);
        await _context.SaveChangesAsync();
        return sentimento;
    }

    public async Task<Sentimentos?> GetByIdAsync(int id)
    {
        return await _context.Sentimentos
            .Include(s => s.Usuario)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sentimentos>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _context.Sentimentos
            .Where(s => s.IdUsuario == usuarioId)
            .OrderByDescending(s => s.Data)
            .ToListAsync();
    }

    public async Task<bool> JaRegistrouHojeAsync(int usuarioId)
    {
        var hoje = DateTime.Today;
        var amanha = hoje.AddDays(1);
        
        var count = await _context.Sentimentos
            .Where(s => s.IdUsuario == usuarioId && s.Data >= hoje && s.Data < amanha)
            .CountAsync();
            
        return count > 0;
    }

    public async Task<decimal?> GetPontuacaoMediaAsync(int usuarioId, DateTime dataInicio, DateTime dataFim)
    {
        var sentimentos = await _context.Sentimentos
            .Where(s => s.IdUsuario == usuarioId 
                && s.Data >= dataInicio 
                && s.Data < dataFim
                && s.ValorPontuacao.HasValue)
            .Select(s => s.ValorPontuacao!.Value)
            .ToListAsync();

        return sentimentos.Any() ? sentimentos.Average() : null;
    }

    public async Task<decimal?> GetPontuacaoMediaEquipeAsync(int equipeId, DateTime dataInicio, DateTime dataFim)
    {
        var sentimentos = await _context.Sentimentos
            .Include(s => s.Usuario)
            .Where(s => s.Usuario.IdEquipe == equipeId
                && s.Data >= dataInicio 
                && s.Data < dataFim
                && s.ValorPontuacao.HasValue)
            .Select(s => s.ValorPontuacao!.Value)
            .ToListAsync();

        return sentimentos.Any() ? sentimentos.Average() : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sentimento = await _context.Sentimentos.FindAsync(id);
        if (sentimento == null)
            return false;

        _context.Sentimentos.Remove(sentimento);
        await _context.SaveChangesAsync();
        return true;
    }
}
