using AuraPlus.Web.Models.DTOs.Reconhecimento;

namespace AuraPlus.Web.Services;

public interface IReconhecimentoService
{
    Task<ReconhecimentoDTO> CreateReconhecimentoAsync(int reconhecedorId, CreateReconhecimentoDTO dto);
    Task<ReconhecimentoEmMassaResultDTO> CreateReconhecimentoEmMassaAsync(int reconhecedorId, CreateReconhecimentoEmMassaDTO dto);
    Task<ReconhecimentoDTO?> GetReconhecimentoByIdAsync(int id);
    Task<IEnumerable<ReconhecimentoDTO>> GetReconhecimentosEnviadosAsync(int usuarioId);
    Task<IEnumerable<ReconhecimentoDTO>> GetReconhecimentosRecebidosAsync(int usuarioId);
    Task<bool> DeleteReconhecimentoAsync(int id, int usuarioId);
}
