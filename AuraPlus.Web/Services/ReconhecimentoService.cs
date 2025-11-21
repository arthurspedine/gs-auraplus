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

    public async Task<ReconhecimentoEmMassaResultDTO> CreateReconhecimentoEmMassaAsync(int reconhecedorId, CreateReconhecimentoEmMassaDTO dto)
    {
        var result = new ReconhecimentoEmMassaResultDTO();

        // Validar reconhecedor uma vez
        var reconhecedor = await _userRepository.GetByIdAsync(reconhecedorId);
        if (reconhecedor == null)
            throw new KeyNotFoundException("Reconhecedor não encontrado.");

        if (reconhecedor.Ativo == '0')
            throw new InvalidOperationException("Reconhecedor está inativo.");

        if (!reconhecedor.IdEquipe.HasValue)
            throw new InvalidOperationException("Você deve estar em uma equipe para fazer reconhecimentos.");

        // Processar cada reconhecimento
        foreach (var item in dto.Reconhecimentos)
        {
            var detalhe = new ReconhecimentoDetalheDTO
            {
                IdReceptor = item.IdReceptor
            };

            try
            {
                // Validar reconhecido
                var reconhecido = await _userRepository.GetByIdAsync(item.IdReceptor);
                if (reconhecido == null)
                {
                    detalhe.Status = "falha";
                    detalhe.Erro = "Usuário reconhecido não encontrado";
                    result.Detalhes.Add(detalhe);
                    result.Falhas++;
                    continue;
                }

                if (reconhecido.Ativo == '0')
                {
                    detalhe.Status = "falha";
                    detalhe.Erro = "Usuário reconhecido está inativo";
                    result.Detalhes.Add(detalhe);
                    result.Falhas++;
                    continue;
                }

                // Validar que o reconhecido está na mesma equipe
                if (reconhecido.IdEquipe != reconhecedor.IdEquipe)
                {
                    detalhe.Status = "falha";
                    detalhe.Erro = "Só é possível reconhecer membros da sua equipe";
                    result.Detalhes.Add(detalhe);
                    result.Falhas++;
                    continue;
                }

                // Não pode reconhecer a si mesmo
                if (reconhecedorId == item.IdReceptor)
                {
                    detalhe.Status = "falha";
                    detalhe.Erro = "Você não pode reconhecer a si mesmo";
                    result.Detalhes.Add(detalhe);
                    result.Falhas++;
                    continue;
                }

                // Regra: mesma pessoa 1x por mês
                var agora = DateTime.Now;
                if (await _reconhecimentoRepository.JaReconheceuPessoaNoMesAsync(reconhecedorId, item.IdReceptor, agora))
                {
                    detalhe.Status = "falha";
                    detalhe.Erro = "Você já reconheceu esta pessoa este mês";
                    result.Detalhes.Add(detalhe);
                    result.Falhas++;
                    continue;
                }

                // Criar reconhecimento
                var reconhecimento = new Reconhecimento
                {
                    Titulo = item.Titulo,
                    Descricao = item.Descricao,
                    Data = DateTime.Now,
                    IdReconhecedor = reconhecedorId,
                    IdReconhecido = item.IdReceptor
                };

                reconhecimento = await _reconhecimentoRepository.AddAsync(reconhecimento);

                detalhe.Status = "sucesso";
                detalhe.Id = reconhecimento.Id;
                result.Detalhes.Add(detalhe);
                result.Sucessos++;

                _logger.LogInformation("Reconhecimento em massa: {Reconhecedor} reconheceu {Reconhecido}", 
                    reconhecedor.Nome, reconhecido.Nome);
            }
            catch (Exception ex)
            {
                detalhe.Status = "falha";
                detalhe.Erro = $"Erro inesperado: {ex.Message}";
                result.Detalhes.Add(detalhe);
                result.Falhas++;
                _logger.LogError(ex, "Erro ao processar reconhecimento em massa para receptor {ReceptorId}", item.IdReceptor);
            }
        }

        return result;
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
