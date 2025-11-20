using AuraPlus.Web.Models;

namespace AuraPlus.Web.Repositories;

public interface ISentimentosRepository
{
    Task<Sentimentos> AddAsync(Sentimentos sentimento);
    Task<Sentimentos?> GetByIdAsync(int id);
    Task<IEnumerable<Sentimentos>> GetByUsuarioIdAsync(int usuarioId);
    Task<bool> JaRegistrouHojeAsync(int usuarioId);
    Task<decimal?> GetPontuacaoMediaAsync(int usuarioId, DateTime dataInicio, DateTime dataFim);
    Task<decimal?> GetPontuacaoMediaEquipeAsync(int equipeId, DateTime dataInicio, DateTime dataFim);
    Task<bool> DeleteAsync(int id);
}
