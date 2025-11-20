using AuraPlus.Web.Models.DTOs.Sentimentos;

namespace AuraPlus.Web.Services;

public interface ISentimentosService
{
    Task<SentimentoDTO> CreateSentimentoAsync(int usuarioId, CreateSentimentoDTO dto);
    Task<SentimentoDTO?> GetSentimentoByIdAsync(int id);
    Task<IEnumerable<SentimentoDTO>> GetSentimentosUsuarioAsync(int usuarioId);
    Task<bool> DeleteSentimentoAsync(int id, int usuarioId);
}
