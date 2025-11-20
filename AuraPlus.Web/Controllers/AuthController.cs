using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuraPlus.Web.Models.DTOs.Auth;
using AuraPlus.Web.Models.DTOs.User;
using AuraPlus.Web.Services;
using System.Net;

namespace AuraPlus.Web.Controllers;

/// <summary>
/// Controller para autenticação e gerenciamento de usuários
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    /// <param name="registerDto">Dados de registro</param>
    /// <returns>Token JWT e informações do usuário</returns>
    /// <response code="201">Usuário registrado com sucesso</response>
    /// <response code="400">Dados inválidos ou email já em uso</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<AuthResponseDTO>> Register(RegisterRequestDTO registerDto)
    {
        try
        {
            var response = await _authService.RegisterAsync(registerDto);
            return CreatedAtAction(nameof(GetMe), response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar usuário");
            return StatusCode(500, new { message = "Erro interno ao registrar usuário" });
        }
    }

    /// <summary>
    /// Autentica um usuário existente
    /// </summary>
    /// <param name="loginDto">Credenciais de login</param>
    /// <returns>Token JWT e informações do usuário</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="401">Credenciais inválidas ou usuário inativo</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<AuthResponseDTO>> Login(LoginRequestDTO loginDto)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer login");
            return StatusCode(500, new { message = "Erro interno ao fazer login" });
        }
    }

    /// <summary>
    /// Obtém informações do usuário autenticado
    /// </summary>
    /// <returns>Informações do usuário</returns>
    /// <response code="200">Informações retornadas com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UserInfoDTO>> GetMe()
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar informações do usuário");
            return StatusCode(500, new { message = "Erro interno ao buscar usuário" });
        }
    }

    /// <summary>
    /// Atualiza informações do usuário autenticado
    /// </summary>
    /// <param name="updateDto">Dados para atualização</param>
    /// <returns>Informações atualizadas do usuário</returns>
    /// <response code="200">Usuário atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UserInfoDTO>> UpdateMe(UpdateUserDTO updateDto)
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var user = await _authService.UpdateUserAsync(userId, updateDto);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário");
            return StatusCode(500, new { message = "Erro interno ao atualizar usuário" });
        }
    }

    /// <summary>
    /// Desativa o usuário autenticado (soft delete)
    /// </summary>
    /// <returns>Status da operação</returns>
    /// <response code="204">Usuário desativado com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpDelete("me")]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteMe()
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var deleted = await _authService.SoftDeleteUserAsync(userId);
            
            if (!deleted)
                return NotFound(new { message = "Usuário não encontrado" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar usuário");
            return StatusCode(500, new { message = "Erro interno ao desativar usuário" });
        }
    }

    /// <summary>
    /// Obtém o ID do usuário autenticado a partir do token JWT
    /// </summary>
    private int GetAuthenticatedUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Token inválido ou usuário não identificado.");
        }

        return userId;
    }
}
