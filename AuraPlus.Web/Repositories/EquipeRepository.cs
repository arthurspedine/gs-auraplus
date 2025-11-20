using Microsoft.EntityFrameworkCore;
using AuraPlus.Web.Data;
using AuraPlus.Web.Models;

namespace AuraPlus.Web.Repositories;

public class EquipeRepository : IEquipeRepository
{
    private readonly OracleDbContext _context;

    public EquipeRepository(OracleDbContext context)
    {
        _context = context;
    }

    public async Task<Equipe?> GetByIdAsync(int id)
    {
        return await _context.Equipes
            .Include(e => e.Users.Where(u => u.Ativo == '1'))
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Equipe>> GetAllAsync()
    {
        return await _context.Equipes
            .Include(e => e.Users.Where(u => u.Ativo == '1'))
            .ToListAsync();
    }

    public async Task<Equipe> AddAsync(Equipe equipe)
    {
        await _context.Equipes.AddAsync(equipe);
        await _context.SaveChangesAsync();
        return equipe;
    }

    public async Task UpdateAsync(Equipe equipe)
    {
        _context.Equipes.Update(equipe);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var equipe = await _context.Equipes.FindAsync(id);
        if (equipe != null)
        {
            _context.Equipes.Remove(equipe);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasMembrosAsync(int id)
    {
        var count = await _context.Users
            .Where(u => u.IdEquipe == id && u.Ativo == '1')
            .CountAsync();
        return count > 0;
    }

    public async Task<int> GetTotalMembrosAsync(int id)
    {
        return await _context.Users
            .CountAsync(u => u.IdEquipe == id && u.Ativo == '1');
    }
}
