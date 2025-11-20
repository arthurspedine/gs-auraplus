using AuraPlus.Web.Models;

namespace AuraPlus.Web.Repositories;

public interface IReconhecimentoRepository
{
    Task<Reconhecimento> AddAsync(Reconhecimento reconhecimento);
    Task<Reconhecimento?> GetByIdAsync(int id);
    Task<IEnumerable<Reconhecimento>> GetByReconhecedorIdAsync(int reconhecedorId);
    Task<IEnumerable<Reconhecimento>> GetByReconhecidoIdAsync(int reconhecidoId);
    Task<bool> JaReconheceuHojeAsync(int reconhecedorId);
    Task<bool> JaReconheceuPessoaNoMesAsync(int reconhecedorId, int reconhecidoId, DateTime mes);
    Task<int> GetTotalReconhecimentosRecebidosAsync(int usuarioId, DateTime dataInicio, DateTime dataFim);
    Task<bool> DeleteAsync(int id);
}
