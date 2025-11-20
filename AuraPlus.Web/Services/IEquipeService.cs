using AuraPlus.Web.Models;
using AuraPlus.Web.Models.DTOs.Equipe;

namespace AuraPlus.Web.Services;

public interface IEquipeService
{
    Task<EquipeDTO> CreateEquipeAsync(int userId, CreateEquipeDTO createDto);
    Task<EquipeDTO> EntrarEquipeAsync(int userId, int equipeId, EntrarEquipeDTO entrarDto);
    Task<bool> SairEquipeAsync(int userId);
    Task<EquipeDTO?> GetEquipeByIdAsync(int equipeId);
    Task<IEnumerable<EquipeDTO>> GetAllEquipesAsync();
    Task<EquipeDTO> UpdateEquipeAsync(int equipeId, int userId, UpdateEquipeDTO updateDto);
    Task<bool> DeleteEquipeAsync(int equipeId, int userId);
    Task<EquipeDTO> AdicionarMembroAsync(int gestorId, int membroId, string cargo, DateTime? dataAdmissao = null);
    Task<bool> RemoverMembroAsync(int gestorId, int membroId);
}
