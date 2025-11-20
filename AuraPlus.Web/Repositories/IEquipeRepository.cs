using AuraPlus.Web.Models;

namespace AuraPlus.Web.Repositories;

public interface IEquipeRepository
{
    Task<Equipe?> GetByIdAsync(int id);
    Task<IEnumerable<Equipe>> GetAllAsync();
    Task<Equipe> AddAsync(Equipe equipe);
    Task UpdateAsync(Equipe equipe);
    Task DeleteAsync(int id);
    Task<bool> HasMembrosAsync(int id);
    Task<int> GetTotalMembrosAsync(int id);
}
