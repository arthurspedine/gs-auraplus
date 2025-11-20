namespace AuraPlus.Web.Models.DTOs.Auth;

public class AuthResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public UserInfoDTO User { get; set; } = null!;
}
