using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuraPlus.Web.Models.DTOs.Reconhecimento;
using AuraPlus.Web.Services;
using System.Net;

namespace AuraPlus.Web.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[Produces("application/json")]
public class ReconhecimentoController : ControllerBase
{
    private readonly IReconhecimentoService _reconhecimentoService;
    private readonly ILogger<ReconhecimentoController> _logger;

    public ReconhecimentoController(IReconhecimentoService reconhecimentoService, ILogger<ReconhecimentoController> logger)
    {
        _reconhecimentoService = reconhecimentoService;
        _logger = logger;
    }

    /// <summary>
    /// Criar um reconhecimento (1 por dia, mesma pessoa 1x por mês)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReconhecimentoDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<ReconhecimentoDTO>> CreateReconhecimento(CreateReconhecimentoDTO dto)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var reconhecimento = await _reconhecimentoService.CreateReconhecimentoAsync(usuarioId, dto);
            
            return CreatedAtAction(nameof(GetById), new { id = reconhecimento.Id }, reconhecimento);
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
            _logger.LogError(ex, "Erro ao criar reconhecimento");
            return StatusCode(500, new { message = "Erro interno ao criar reconhecimento" });
        }
    }

    /// <summary>
    /// Obter reconhecimento por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReconhecimentoDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ReconhecimentoDTO>> GetById(int id)
    {
        try
        {
            var reconhecimento = await _reconhecimentoService.GetReconhecimentoByIdAsync(id);
            
            if (reconhecimento == null)
                return NotFound(new { message = "Reconhecimento não encontrado" });

            return Ok(reconhecimento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar reconhecimento");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

    /// <summary>
    /// Listar reconhecimentos enviados pelo usuário autenticado
    /// </summary>
    [HttpGet("enviados")]
    [ProducesResponseType(typeof(IEnumerable<ReconhecimentoDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ReconhecimentoDTO>>> GetEnviados()
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var reconhecimentos = await _reconhecimentoService.GetReconhecimentosEnviadosAsync(usuarioId);
            
            return Ok(reconhecimentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar reconhecimentos enviados");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

    /// <summary>
    /// Listar reconhecimentos recebidos pelo usuário autenticado
    /// </summary>
    [HttpGet("recebidos")]
    [ProducesResponseType(typeof(IEnumerable<ReconhecimentoDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ReconhecimentoDTO>>> GetRecebidos()
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var reconhecimentos = await _reconhecimentoService.GetReconhecimentosRecebidosAsync(usuarioId);
            
            return Ok(reconhecimentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar reconhecimentos recebidos");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

    /// <summary>
    /// Deletar reconhecimento (apenas quem criou)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            await _reconhecimentoService.DeleteReconhecimentoAsync(id, usuarioId);
            
            return NoContent();
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
            _logger.LogError(ex, "Erro ao deletar reconhecimento");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

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
