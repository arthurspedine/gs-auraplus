using AuraPlus.Web.Models;
using AuraPlus.Web.Models.DTOs.Relatorio;
using AuraPlus.Web.Repositories;

namespace AuraPlus.Web.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IRelatorioRepository _relatorioRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IReconhecimentoRepository _reconhecimentoRepository;
    private readonly ISentimentosRepository _sentimentosRepository;
    private readonly ILogger<RelatorioService> _logger;

    public RelatorioService(
        IRelatorioRepository relatorioRepository,
        IUserRepository userRepository,
        IEquipeRepository equipeRepository,
        IReconhecimentoRepository reconhecimentoRepository,
        ISentimentosRepository sentimentosRepository,
        ILogger<RelatorioService> logger)
    {
        _relatorioRepository = relatorioRepository;
        _userRepository = userRepository;
        _equipeRepository = equipeRepository;
        _reconhecimentoRepository = reconhecimentoRepository;
        _sentimentosRepository = sentimentosRepository;
        _logger = logger;
    }

    public async Task<RelatorioPessoaDTO> GerarRelatorioPessoaAsync(int usuarioId)
    {
        var usuario = await _userRepository.GetByIdAsync(usuarioId);
        
        if (usuario == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (usuario.Ativo == '0')
            throw new InvalidOperationException("Usuário está inativo.");

        // Validar que está em uma equipe
        if (!usuario.IdEquipe.HasValue)
            throw new InvalidOperationException("Você deve estar em uma equipe para gerar relatórios.");

        // Período: últimos 30 dias
        var dataFim = DateTime.Now;
        var dataInicio = dataFim.AddDays(-30);

        // Calcular número de reconhecimentos recebidos
        var numeroIndicacoes = await _reconhecimentoRepository.GetTotalReconhecimentosRecebidosAsync(usuarioId, dataInicio, dataFim);

        // Calcular pontuação média de sentimentos
        var pontuacaoMedia = await _sentimentosRepository.GetPontuacaoMediaAsync(usuarioId, dataInicio, dataFim);

        var descritivo = $"Reconhecimentos recebidos: {numeroIndicacoes}. ";
        if (pontuacaoMedia.HasValue)
        {
            descritivo += $"Sentimento médio: {pontuacaoMedia.Value:F2}/10. ";
            
            if (pontuacaoMedia.Value >= 8)
                descritivo += "Excelente engajamento!";
            else if (pontuacaoMedia.Value >= 6)
                descritivo += "Bom engajamento.";
            else if (pontuacaoMedia.Value >= 4)
                descritivo += "Atenção: engajamento moderado.";
            else
                descritivo += "Atenção: baixo engajamento.";
        }
        else
        {
            descritivo += "Nenhum sentimento registrado no período.";
        }

        var relatorio = new RelatorioPessoa
        {
            NumeroIndicacoes = numeroIndicacoes,
            Data = DateTime.Now,
            Descritivo = descritivo,
            IdUsuario = usuarioId
        };

        relatorio = await _relatorioRepository.AddRelatorioPessoaAsync(relatorio);

        _logger.LogInformation("Relatório pessoa gerado: {Usuario} - {Indicacoes} reconhecimentos", 
            usuario.Nome, numeroIndicacoes);

        // Recarregar com navigation properties
        relatorio = await _relatorioRepository.GetRelatorioPessoaByIdAsync(relatorio.Id) ?? relatorio;

        return RelatorioPessoaDTO.FromRelatorioPessoa(relatorio);
    }

    public async Task<RelatorioEquipeDTO> GerarRelatorioEquipeAsync(int equipeId)
    {
        var equipe = await _equipeRepository.GetByIdAsync(equipeId);
        
        if (equipe == null)
            throw new KeyNotFoundException("Equipe não encontrada.");

        var totalMembros = equipe.Users?.Count(u => u.Ativo == '1') ?? 0;

        if (totalMembros == 0)
            throw new InvalidOperationException("Equipe não possui membros ativos.");

        // Período: últimos 30 dias
        var dataFim = DateTime.Now;
        var dataInicio = dataFim.AddDays(-30);

        // Calcular sentimento médio da equipe
        var pontuacaoMedia = await _sentimentosRepository.GetPontuacaoMediaEquipeAsync(equipeId, dataInicio, dataFim);

        string sentimentoMedio;
        string descritivo;

        if (pontuacaoMedia.HasValue)
        {
            sentimentoMedio = pontuacaoMedia.Value >= 8 ? "Excelente" :
                             pontuacaoMedia.Value >= 6 ? "Bom" :
                             pontuacaoMedia.Value >= 4 ? "Regular" : "Crítico";

            descritivo = $"Equipe com {totalMembros} membros. Sentimento médio: {pontuacaoMedia.Value:F2}/10 ({sentimentoMedio}). ";

            if (pontuacaoMedia.Value >= 7)
                descritivo += "Equipe engajada e motivada!";
            else if (pontuacaoMedia.Value >= 5)
                descritivo += "Equipe com engajamento moderado.";
            else
                descritivo += "Atenção: equipe necessita de intervenções para melhorar engajamento.";
        }
        else
        {
            sentimentoMedio = "Sem dados";
            descritivo = $"Equipe com {totalMembros} membros. Nenhum sentimento registrado no período.";
        }

        var relatorio = new RelatorioEquipe
        {
            Data = DateTime.Now,
            SentimentoMedio = sentimentoMedio,
            Descritivo = descritivo,
            IdEquipe = equipeId
        };

        relatorio = await _relatorioRepository.AddRelatorioEquipeAsync(relatorio);

        _logger.LogInformation("Relatório equipe gerado: {Equipe} - Sentimento {Sentimento}", 
            equipe.NmTime, sentimentoMedio);

        // Recarregar com navigation properties
        relatorio = await _relatorioRepository.GetRelatorioEquipeByIdAsync(relatorio.Id) ?? relatorio;

        return RelatorioEquipeDTO.FromRelatorioEquipe(relatorio);
    }

    public async Task<RelatorioPessoaDTO?> GetRelatorioPessoaByIdAsync(int id)
    {
        var relatorio = await _relatorioRepository.GetRelatorioPessoaByIdAsync(id);
        return relatorio != null ? RelatorioPessoaDTO.FromRelatorioPessoa(relatorio) : null;
    }

    public async Task<RelatorioEquipeDTO?> GetRelatorioEquipeByIdAsync(int id)
    {
        var relatorio = await _relatorioRepository.GetRelatorioEquipeByIdAsync(id);
        return relatorio != null ? RelatorioEquipeDTO.FromRelatorioEquipe(relatorio) : null;
    }

    public async Task<IEnumerable<RelatorioPessoaDTO>> GetRelatoriosPessoaUsuarioAsync(int usuarioId)
    {
        var relatorios = await _relatorioRepository.GetRelatoriosPessoaPorUsuarioAsync(usuarioId);
        return relatorios.Select(r => RelatorioPessoaDTO.FromRelatorioPessoa(r));
    }

    public async Task<IEnumerable<RelatorioEquipeDTO>> GetRelatoriosEquipeAsync(int equipeId)
    {
        var relatorios = await _relatorioRepository.GetRelatoriosEquipePorEquipeAsync(equipeId);
        return relatorios.Select(r => RelatorioEquipeDTO.FromRelatorioEquipe(r));
    }
}
