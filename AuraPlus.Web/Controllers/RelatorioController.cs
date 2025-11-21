using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuraPlus.Web.Models.DTOs.Relatorio;
using AuraPlus.Web.Models.Common;
using AuraPlus.Web.Services;
using System.Net;

namespace AuraPlus.Web.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[Produces("application/json")]
public class RelatorioController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;
    private readonly ILogger<RelatorioController> _logger;

    public RelatorioController(IRelatorioService relatorioService, ILogger<RelatorioController> logger)
    {
        _relatorioService = relatorioService;
        _logger = logger;
    }

    /// <summary>
    /// Gerar relatório pessoal do usuário autenticado
    /// </summary>
    [HttpPost("pessoa")]
    [ProducesResponseType(typeof(RelatorioPessoaDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<RelatorioPessoaDTO>> GerarRelatorioPessoa()
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var relatorio = await _relatorioService.GerarRelatorioPessoaAsync(usuarioId);
            
            return CreatedAtAction(nameof(GetRelatorioPessoaById), new { id = relatorio.Id }, relatorio);
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
            _logger.LogError(ex, "Erro ao gerar relatório pessoa");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

    /// <summary>
    /// Gerar relatório da equipe (requer ID da equipe)
    /// </summary>
    [HttpPost("equipe/{equipeId}")]
    [ProducesResponseType(typeof(RelatorioEquipeDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<RelatorioEquipeDTO>> GerarRelatorioEquipe(int equipeId)
    {
        try
        {
            var relatorio = await _relatorioService.GerarRelatorioEquipeAsync(equipeId);
            
            return CreatedAtAction(nameof(GetRelatorioEquipeById), new { id = relatorio.Id }, relatorio);
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
            _logger.LogError(ex, "Erro ao gerar relatório equipe");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

    /// <summary>
    /// Obter relatório pessoa por ID
    /// </summary>
    [HttpGet("pessoa/{id}")]
    [ProducesResponseType(typeof(RelatorioPessoaDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<RelatorioPessoaDTO>> GetRelatorioPessoaById(int id)
    {
        try
        {
            var relatorio = await _relatorioService.GetRelatorioPessoaByIdAsync(id);
            
            if (relatorio == null)
                return NotFound(new { message = "Relatório não encontrado" });

            return Ok(relatorio);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar relatório pessoa");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

    /// <summary>
    /// Obter relatório equipe por ID
    /// </summary>
    [HttpGet("equipe/{id}")]
    [ProducesResponseType(typeof(RelatorioEquipeDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<RelatorioEquipeDTO>> GetRelatorioEquipeById(int id)
    {
        try
        {
            var relatorio = await _relatorioService.GetRelatorioEquipeByIdAsync(id);
            
            if (relatorio == null)
                return NotFound(new { message = "Relatório não encontrado" });

            return Ok(relatorio);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar relatório equipe");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

    /// <summary>
    /// Listar histórico de relatórios pessoais do usuário autenticado com paginação
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10, máximo: 100)</param>
    [HttpGet("pessoa/historico")]
    [ProducesResponseType(typeof(PagedResult<RelatorioPessoaDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PagedResult<RelatorioPessoaDTO>>> GetHistoricoRelatorioPessoa([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var allRelatorios = await _relatorioService.GetRelatoriosPessoaUsuarioAsync(usuarioId);
            var totalCount = allRelatorios.Count();
            
            var pagedData = allRelatorios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResult<RelatorioPessoaDTO>(pagedData, page, pageSize, totalCount);
            
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            var hateoasLinks = new HateoasLinks();
            hateoasLinks.AddPaginationLinks(baseUrl, page, result.TotalPages, pageSize);
            result.Links = hateoasLinks.Links;
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar histórico relatórios pessoa");
            return StatusCode(500, new { message = "Erro interno" });
        }
    }

    /// <summary>
    /// Listar histórico de relatórios da equipe com paginação
    /// </summary>
    /// <param name="equipeId">ID da equipe</param>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10, máximo: 100)</param>
    [HttpGet("equipe/historico/{equipeId}")]
    [ProducesResponseType(typeof(PagedResult<RelatorioEquipeDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PagedResult<RelatorioEquipeDTO>>> GetHistoricoRelatorioEquipe(int equipeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var allRelatorios = await _relatorioService.GetRelatoriosEquipeAsync(equipeId);
            var totalCount = allRelatorios.Count();
            
            var pagedData = allRelatorios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResult<RelatorioEquipeDTO>(pagedData, page, pageSize, totalCount);
            
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            var hateoasLinks = new HateoasLinks();
            hateoasLinks.AddPaginationLinks(baseUrl, page, result.TotalPages, pageSize);
            result.Links = hateoasLinks.Links;
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar histórico relatórios equipe");
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
