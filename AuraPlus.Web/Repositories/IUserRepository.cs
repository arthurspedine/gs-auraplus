using AuraPlus.Web.Models;

namespace AuraPlus.Web.Repositories;

public interface IUserRepository
{
    Task<Users?> GetByIdAsync(int id);
    Task<Users?> GetByEmailAsync(string email);
    Task<IEnumerable<Users>> GetAllAsync();
    Task<Users> AddAsync(Users user);
    Task UpdateAsync(Users user);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> EmailExistsAsync(string email);
}
