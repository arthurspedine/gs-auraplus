using AuraPlus.Web.Models;
using AuraPlus.Web.Models.DTOs.Reconhecimento;
using AuraPlus.Web.Repositories;

namespace AuraPlus.Web.Services;

public class ReconhecimentoService : IReconhecimentoService
{
    private readonly IReconhecimentoRepository _reconhecimentoRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ReconhecimentoService> _logger;

    public ReconhecimentoService(
        IReconhecimentoRepository reconhecimentoRepository,
        IUserRepository userRepository,
        ILogger<ReconhecimentoService> logger)
    {
        _reconhecimentoRepository = reconhecimentoRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ReconhecimentoDTO> CreateReconhecimentoAsync(int reconhecedorId, CreateReconhecimentoDTO dto)
    {
        // Validar reconhecedor
        var reconhecedor = await _userRepository.GetByIdAsync(reconhecedorId);
        if (reconhecedor == null)
            throw new KeyNotFoundException("Reconhecedor não encontrado.");

        if (reconhecedor.Ativo == '0')
            throw new InvalidOperationException("Reconhecedor está inativo.");

        // Validar que está em uma equipe
        if (!reconhecedor.IdEquipe.HasValue)
            throw new InvalidOperationException("Você deve estar em uma equipe para fazer reconhecimentos.");

        // Validar reconhecido
        var reconhecido = await _userRepository.GetByIdAsync(dto.IdReconhecido);
        if (reconhecido == null)
            throw new KeyNotFoundException("Usuário reconhecido não encontrado.");

        if (reconhecido.Ativo == '0')
            throw new InvalidOperationException("Usuário reconhecido está inativo.");

        // Validar que o reconhecido está na mesma equipe
        if (reconhecido.IdEquipe != reconhecedor.IdEquipe)
            throw new InvalidOperationException("Só é possível reconhecer membros da sua equipe.");

        // Não pode reconhecer a si mesmo
        if (reconhecedorId == dto.IdReconhecido)
            throw new InvalidOperationException("Você não pode reconhecer a si mesmo.");

        // Regra: 1 reconhecimento por dia
        if (await _reconhecimentoRepository.JaReconheceuHojeAsync(reconhecedorId))
            throw new InvalidOperationException("Você já fez um reconhecimento hoje. Apenas 1 reconhecimento por dia é permitido.");

        // Regra: mesma pessoa 1x por mês
        var agora = DateTime.Now;
        if (await _reconhecimentoRepository.JaReconheceuPessoaNoMesAsync(reconhecedorId, dto.IdReconhecido, agora))
            throw new InvalidOperationException("Você já reconheceu esta pessoa este mês. Apenas 1 reconhecimento por pessoa por mês é permitido.");

        var reconhecimento = new Reconhecimento
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Data = DateTime.Now,
            IdReconhecedor = reconhecedorId,
            IdReconhecido = dto.IdReconhecido
        };

        reconhecimento = await _reconhecimentoRepository.AddAsync(reconhecimento);

        _logger.LogInformation("Reconhecimento criado: {Reconhecedor} reconheceu {Reconhecido}", 
            reconhecedor.Nome, reconhecido.Nome);

        // Recarregar com navigation properties
        reconhecimento = await _reconhecimentoRepository.GetByIdAsync(reconhecimento.Id) ?? reconhecimento;

        return ReconhecimentoDTO.FromReconhecimento(reconhecimento);
    }

    public async Task<ReconhecimentoDTO?> GetReconhecimentoByIdAsync(int id)
    {
        var reconhecimento = await _reconhecimentoRepository.GetByIdAsync(id);
        return reconhecimento != null ? ReconhecimentoDTO.FromReconhecimento(reconhecimento) : null;
    }

    public async Task<IEnumerable<ReconhecimentoDTO>> GetReconhecimentosEnviadosAsync(int usuarioId)
    {
        var reconhecimentos = await _reconhecimentoRepository.GetByReconhecedorIdAsync(usuarioId);
        return reconhecimentos.Select(r => ReconhecimentoDTO.FromReconhecimento(r));
    }

    public async Task<IEnumerable<ReconhecimentoDTO>> GetReconhecimentosRecebidosAsync(int usuarioId)
    {
        var reconhecimentos = await _reconhecimentoRepository.GetByReconhecidoIdAsync(usuarioId);
        return reconhecimentos.Select(r => ReconhecimentoDTO.FromReconhecimento(r));
    }

    public async Task<bool> DeleteReconhecimentoAsync(int id, int usuarioId)
    {
        var reconhecimento = await _reconhecimentoRepository.GetByIdAsync(id);
        
        if (reconhecimento == null)
            throw new KeyNotFoundException("Reconhecimento não encontrado.");

        // Apenas o reconhecedor pode deletar
        if (reconhecimento.IdReconhecedor != usuarioId)
            throw new UnauthorizedAccessException("Apenas quem fez o reconhecimento pode deletá-lo.");

        var result = await _reconhecimentoRepository.DeleteAsync(id);

        if (result)
            _logger.LogInformation("Reconhecimento deletado: ID {ReconhecimentoId}", id);

        return result;
    }
}
