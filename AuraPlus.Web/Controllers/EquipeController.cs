using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuraPlus.Web.Models.DTOs.Equipe;
using AuraPlus.Web.Services;
using System.Net;

namespace AuraPlus.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de equipes
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[Produces("application/json")]
public class EquipeController : ControllerBase
{
    private readonly IEquipeService _equipeService;
    private readonly ILogger<EquipeController> _logger;

    public EquipeController(IEquipeService equipeService, ILogger<EquipeController> logger)
    {
        _equipeService = equipeService;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova equipe e torna o usuário GESTOR
    /// </summary>
    /// <param name="createDto">Dados da equipe a ser criada</param>
    /// <returns>Equipe criada</returns>
    /// <response code="201">Equipe criada com sucesso</response>
    /// <response code="400">Usuário já está em uma equipe</response>
    /// <response code="401">Não autorizado</response>
    [HttpPost]
    [ProducesResponseType(typeof(EquipeDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<EquipeDTO>> CreateEquipe(CreateEquipeDTO createDto)
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var equipe = await _equipeService.CreateEquipeAsync(userId, createDto);
            
            return CreatedAtAction(nameof(GetById), new { id = equipe.Id }, equipe);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar equipe");
            return StatusCode(500, new { message = "Erro interno ao criar equipe" });
        }
    }

    /// <summary>
    /// Lista todas as equipes
    /// </summary>
    /// <returns>Lista de equipes</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EquipeDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<IEnumerable<EquipeDTO>>> GetAll()
    {
        try
        {
            var equipes = await _equipeService.GetAllEquipesAsync();
            return Ok(equipes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar equipes");
            return StatusCode(500, new { message = "Erro interno ao listar equipes" });
        }
    }

    /// <summary>
    /// Obtém uma equipe por ID
    /// </summary>
    /// <param name="id">ID da equipe</param>
    /// <returns>Dados da equipe</returns>
    /// <response code="200">Equipe encontrada</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Equipe não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EquipeDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<EquipeDTO>> GetById(int id)
    {
        try
        {
            var equipe = await _equipeService.GetEquipeByIdAsync(id);
            
            if (equipe == null)
                return NotFound(new { message = "Equipe não encontrada" });

            return Ok(equipe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar equipe");
            return StatusCode(500, new { message = "Erro interno ao buscar equipe" });
        }
    }

    /// <summary>
    /// Usuário entra em uma equipe existente e vira EMPREGADO
    /// </summary>
    /// <param name="id">ID da equipe</param>
    /// <param name="entrarDto">Dados para entrada na equipe</param>
    /// <returns>Dados da equipe atualizada</returns>
    /// <response code="200">Entrada na equipe realizada com sucesso</response>
    /// <response code="400">Usuário já está em uma equipe</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Equipe não encontrada</response>
    [HttpPost("{id}/entrar")]
    [ProducesResponseType(typeof(EquipeDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<EquipeDTO>> EntrarEquipe(int id, EntrarEquipeDTO entrarDto)
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var equipe = await _equipeService.EntrarEquipeAsync(userId, id, entrarDto);
            
            return Ok(equipe);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao entrar na equipe");
            return StatusCode(500, new { message = "Erro interno ao entrar na equipe" });
        }
    }

    /// <summary>
    /// Usuário sai da equipe atual
    /// </summary>
    /// <returns>Status da operação</returns>
    /// <response code="204">Saída da equipe realizada com sucesso</response>
    /// <response code="400">Usuário não está em uma equipe</response>
    /// <response code="401">Não autorizado</response>
    [HttpPost("sair")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> SairEquipe()
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            await _equipeService.SairEquipeAsync(userId);
            
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sair da equipe");
            return StatusCode(500, new { message = "Erro interno ao sair da equipe" });
        }
    }

    /// <summary>
    /// Atualiza informações da equipe (apenas GESTOR)
    /// </summary>
    /// <param name="id">ID da equipe</param>
    /// <param name="updateDto">Dados para atualização</param>
    /// <returns>Dados da equipe atualizada</returns>
    /// <response code="200">Equipe atualizada com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="403">Apenas o gestor pode atualizar a equipe</response>
    /// <response code="404">Equipe não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(EquipeDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<EquipeDTO>> UpdateEquipe(int id, UpdateEquipeDTO updateDto)
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var equipe = await _equipeService.UpdateEquipeAsync(id, userId, updateDto);
            
            return Ok(equipe);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar equipe");
            return StatusCode(500, new { message = "Erro interno ao atualizar equipe" });
        }
    }

    /// <summary>
    /// Deleta a equipe (apenas GESTOR e se não houver outros membros)
    /// </summary>
    /// <param name="id">ID da equipe</param>
    /// <returns>Status da operação</returns>
    /// <response code="204">Equipe deletada com sucesso</response>
    /// <response code="400">Equipe possui outros membros</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="403">Apenas o gestor pode deletar a equipe</response>
    /// <response code="404">Equipe não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteEquipe(int id)
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            await _equipeService.DeleteEquipeAsync(id, userId);
            
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar equipe");
            return StatusCode(500, new { message = "Erro interno ao deletar equipe" });
        }
    }

    /// <summary>
    /// Gestor adiciona um membro à sua equipe
    /// </summary>
    /// <param name="dto">Dados do membro a ser adicionado</param>
    /// <returns>Dados da equipe atualizada</returns>
    /// <response code="200">Membro adicionado com sucesso</response>
    /// <response code="400">Membro já está em uma equipe</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="403">Apenas gestores podem adicionar membros</response>
    /// <response code="404">Membro não encontrado</response>
    [HttpPost("membros")]
    [ProducesResponseType(typeof(EquipeDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<EquipeDTO>> AdicionarMembro(AdicionarMembroDTO dto)
    {
        try
        {
            var gestorId = GetAuthenticatedUserId();
            
            // Valida se é gestor através do JWT
            if (!IsGestor())
                return StatusCode(403, new { message = "Apenas gestores podem adicionar membros à equipe" });
            
            var equipe = await _equipeService.AdicionarMembroAsync(gestorId, dto.MembroId, dto.Cargo, dto.DataAdmissao);
            
            return Ok(equipe);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar membro à equipe");
            return StatusCode(500, new { message = "Erro interno ao adicionar membro" });
        }
    }

    /// <summary>
    /// Gestor remove um membro da sua equipe
    /// </summary>
    /// <param name="membroId">ID do membro a ser removido</param>
    /// <returns>Status da operação</returns>
    /// <response code="204">Membro removido com sucesso</response>
    /// <response code="400">Membro não pertence à equipe</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="403">Apenas gestores podem remover membros</response>
    /// <response code="404">Membro não encontrado</response>
    [HttpDelete("membros/{membroId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> RemoverMembro(int membroId)
    {
        try
        {
            var gestorId = GetAuthenticatedUserId();
            
            // Valida se é gestor através do JWT
            if (!IsGestor())
                return StatusCode(403, new { message = "Apenas gestores podem remover membros da equipe" });
            
            await _equipeService.RemoverMembroAsync(gestorId, membroId);
            
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover membro da equipe");
            return StatusCode(500, new { message = "Erro interno ao remover membro" });
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

    /// <summary>
    /// Verifica se o usuário autenticado é GESTOR através do role no JWT
    /// </summary>
    private bool IsGestor()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        return roleClaim?.ToUpper() == "GESTOR";
    }
}
