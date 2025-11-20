using AuraPlus.Web.Models;
using AuraPlus.Web.Models.DTOs.Auth;
using AuraPlus.Web.Models.DTOs.User;

namespace AuraPlus.Web.Services;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO registerDto);
    Task<AuthResponseDTO> LoginAsync(LoginRequestDTO loginDto);
    Task<UserInfoDTO?> GetUserByIdAsync(int userId);
    Task<UserInfoDTO> UpdateUserAsync(int userId, UpdateUserDTO updateDto);
    Task<bool> SoftDeleteUserAsync(int userId);
    Task<bool> IsUserActiveAsync(int userId);
}
