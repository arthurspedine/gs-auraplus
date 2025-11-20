using AuraPlus.Web.Models;
using AuraPlus.Web.Models.DTOs.Sentimentos;
using AuraPlus.Web.Repositories;

namespace AuraPlus.Web.Services;

public class SentimentosService : ISentimentosService
{
    private readonly ISentimentosRepository _sentimentosRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SentimentosService> _logger;

    public SentimentosService(
        ISentimentosRepository sentimentosRepository,
        IUserRepository userRepository,
        ILogger<SentimentosService> logger)
    {
        _sentimentosRepository = sentimentosRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<SentimentoDTO> CreateSentimentoAsync(int usuarioId, CreateSentimentoDTO dto)
    {
        var usuario = await _userRepository.GetByIdAsync(usuarioId);
        
        if (usuario == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (usuario.Ativo == '0')
            throw new InvalidOperationException("Usuário está inativo.");

        // Validar que está em uma equipe
        if (!usuario.IdEquipe.HasValue)
            throw new InvalidOperationException("Você deve estar em uma equipe para registrar sentimentos.");

        // Regra: 1 sentimento por dia
        if (await _sentimentosRepository.JaRegistrouHojeAsync(usuarioId))
            throw new InvalidOperationException("Você já registrou um sentimento hoje. Apenas 1 registro por dia é permitido.");

        var sentimento = new Sentimentos
        {
            NomeSentimento = dto.NomeSentimento,
            ValorPontuacao = dto.ValorPontuacao,
            Descricao = dto.Descricao,
            Data = DateTime.Now,
            IdUsuario = usuarioId
        };

        sentimento = await _sentimentosRepository.AddAsync(sentimento);

        _logger.LogInformation("Sentimento registrado: {Usuario} - {Sentimento} ({Pontuacao})", 
            usuario.Nome, sentimento.NomeSentimento, sentimento.ValorPontuacao);

        // Recarregar com navigation properties
        sentimento = await _sentimentosRepository.GetByIdAsync(sentimento.Id) ?? sentimento;

        return SentimentoDTO.FromSentimento(sentimento);
    }

    public async Task<SentimentoDTO?> GetSentimentoByIdAsync(int id)
    {
        var sentimento = await _sentimentosRepository.GetByIdAsync(id);
        return sentimento != null ? SentimentoDTO.FromSentimento(sentimento) : null;
    }

    public async Task<IEnumerable<SentimentoDTO>> GetSentimentosUsuarioAsync(int usuarioId)
    {
        var sentimentos = await _sentimentosRepository.GetByUsuarioIdAsync(usuarioId);
        return sentimentos.Select(s => SentimentoDTO.FromSentimento(s));
    }

    public async Task<bool> DeleteSentimentoAsync(int id, int usuarioId)
    {
        var sentimento = await _sentimentosRepository.GetByIdAsync(id);
        
        if (sentimento == null)
            throw new KeyNotFoundException("Sentimento não encontrado.");

        // Apenas o próprio usuário pode deletar
        if (sentimento.IdUsuario != usuarioId)
            throw new UnauthorizedAccessException("Você só pode deletar seus próprios sentimentos.");

        var result = await _sentimentosRepository.DeleteAsync(id);

        if (result)
            _logger.LogInformation("Sentimento deletado: ID {SentimentoId}", id);

        return result;
    }
}
