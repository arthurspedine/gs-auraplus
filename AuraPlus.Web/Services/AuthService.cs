using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AuraPlus.Web.Models;
using AuraPlus.Web.Models.DTOs.Auth;
using AuraPlus.Web.Models.DTOs.User;
using AuraPlus.Web.Repositories;

namespace AuraPlus.Web.Services;

/// <summary>
/// Serviço de autenticação e gerenciamento de usuários
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO registerDto)
    {
        // Verifica se o email já está em uso
        if (await _userRepository.EmailExistsAsync(registerDto.Email))
        {
            throw new InvalidOperationException("Email já está em uso.");
        }

        // Cria o hash da senha usando BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password, workFactor: 12);

        // Cria o novo usuário com role NOVO_USUARIO
        var user = new Users
        {
            Nome = registerDto.Nome,
            Email = registerDto.Email.ToLower(),
            Senha = passwordHash,
            Role = "NOVO_USUARIO",
            Ativo = '1'
        };

        user = await _userRepository.AddAsync(user);

        _logger.LogInformation("Novo usuário registrado: {Nome} ({Email})", user.Nome, user.Email);

        // Gera o token JWT
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes());

        return new AuthResponseDTO
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresAt = expiresAt,
            User = UserInfoDTO.FromUser(user)
        };
    }

    /// <summary>
    /// Autentica um usuário existente
    /// </summary>
    public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO loginDto)
    {
        // Busca o usuário pelo email
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);

        if (user == null)
        {
            _logger.LogWarning("Tentativa de login com email inexistente: {Email}", loginDto.Email);
            throw new UnauthorizedAccessException("Email ou senha inválidos.");
        }

        // Verifica se o usuário está ativo
        if (user.Ativo == '0')
        {
            _logger.LogWarning("Tentativa de login de usuário inativo: {Email}", loginDto.Email);
            throw new UnauthorizedAccessException("Usuário inativo.");
        }

        // Verifica a senha usando BCrypt
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Senha);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Tentativa de login com senha incorreta para: {Email}", loginDto.Email);
            throw new UnauthorizedAccessException("Email ou senha inválidos.");
        }

        _logger.LogInformation("Login bem-sucedido: {Nome} ({Email})", user.Nome, user.Email);

        // Gera o token JWT
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes());

        return new AuthResponseDTO
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresAt = expiresAt,
            User = UserInfoDTO.FromUser(user)
        };
    }

    /// <summary>
    /// Busca um usuário pelo ID
    /// </summary>
    public async Task<UserInfoDTO?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? UserInfoDTO.FromUser(user) : null;
    }

    /// <summary>
    /// Atualiza informações do usuário
    /// </summary>
    public async Task<UserInfoDTO> UpdateUserAsync(int userId, UpdateUserDTO updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (user.Ativo == '0')
            throw new InvalidOperationException("Usuário está inativo.");

        // Atualiza campos se fornecidos
        if (!string.IsNullOrWhiteSpace(updateDto.Nome))
            user.Nome = updateDto.Nome;

        if (!string.IsNullOrWhiteSpace(updateDto.Email))
        {
            if (user.Email != updateDto.Email.ToLower() 
                && await _userRepository.EmailExistsAsync(updateDto.Email))
            {
                throw new InvalidOperationException("Email já está em uso.");
            }
            user.Email = updateDto.Email.ToLower();
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Password))
        {
            user.Senha = BCrypt.Net.BCrypt.HashPassword(updateDto.Password, workFactor: 12);
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Cargo))
            user.Cargo = updateDto.Cargo;

        if (updateDto.DataAdmissao.HasValue)
            user.DataAdmissao = updateDto.DataAdmissao;

        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Usuário atualizado: {Nome} (ID: {Id})", user.Nome, user.Id);

        return UserInfoDTO.FromUser(user);
    }

    /// <summary>
    /// Soft delete - Desativa usuário (Ativo = '0')
    /// </summary>
    public async Task<bool> SoftDeleteUserAsync(int userId)
    {
        var result = await _userRepository.SoftDeleteAsync(userId);
        
        if (result)
        {
            _logger.LogInformation("Usuário desativado: ID {UserId}", userId);
        }

        return result;
    }

    /// <summary>
    /// Verifica se um usuário está ativo
    /// </summary>
    public async Task<bool> IsUserActiveAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null && user.Ativo == '1';
    }

    /// <summary>
    /// Gera um token JWT para o usuário
    /// </summary>
    private string GenerateJwtToken(Users user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey não configurada.");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Nome),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nome),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Obtém o tempo de expiração do token em minutos
    /// </summary>
    private int GetTokenExpirationMinutes()
    {
        var expirationMinutes = _configuration.GetSection("JwtSettings")["ExpirationMinutes"];
        return int.TryParse(expirationMinutes, out var minutes) ? minutes : 60;
    }
}
