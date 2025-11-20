using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuraPlus.Web.Models.DTOs.Sentimentos;
using AuraPlus.Web.Services;
using System.Net;

namespace AuraPlus.Web.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[Produces("application/json")]
public class SentimentosController : ControllerBase
{
    private readonly ISentimentosService _sentimentosService;
    private readonly ILogger<SentimentosController> _logger;

    public SentimentosController(ISentimentosService sentimentosService, ILogger<SentimentosController> logger)
    {
        _sentimentosService = sentimentosService;
        _logger = logger;
    }

    /// <summary>
    /// Registrar sentimento do dia (1 por dia)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SentimentoDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<SentimentoDTO>> CreateSentimento(CreateSentimentoDTO dto)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var sentimento = await _sentimentosService.CreateSentimentoAsync(usuarioId, dto);
            
            return CreatedAtAction(nameof(GetById), new { id = sentimento.Id }, sentimento);
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
            _logger.LogError(ex, "Erro ao criar sentimento");
            return StatusCode(500, new { message = "Erro interno ao criar sentimento" });
        }
    }

    /// <summary>
    /// Obter sentimento por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SentimentoDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<SentimentoDTO>> GetById(int id)
    {
        try
        {
            var sentimento = await _sentimentosService.GetSentimentoByIdAsync(id);
            
            if (sentimento == null)
                return NotFound(new { message = "Sentimento não encontrado" });

            return Ok(sentimento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sentimento");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

    /// <summary>
    /// Listar todos os sentimentos do usuário autenticado
    /// </summary>
    [HttpGet("meus")]
    [ProducesResponseType(typeof(IEnumerable<SentimentoDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<SentimentoDTO>>> GetMeusSentimentos()
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var sentimentos = await _sentimentosService.GetSentimentosUsuarioAsync(usuarioId);
            
            return Ok(sentimentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar sentimentos");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

    /// <summary>
    /// Deletar sentimento (apenas próprio usuário)
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
            await _sentimentosService.DeleteSentimentoAsync(id, usuarioId);
            
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
            _logger.LogError(ex, "Erro ao deletar sentimento");
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
