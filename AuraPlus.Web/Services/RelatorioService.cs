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
    private readonly MLPredictionService _mlPredictionService;
    private readonly ILogger<RelatorioService> _logger;

    public RelatorioService(
        IRelatorioRepository relatorioRepository,
        IUserRepository userRepository,
        IEquipeRepository equipeRepository,
        IReconhecimentoRepository reconhecimentoRepository,
        ISentimentosRepository sentimentosRepository,
        MLPredictionService mlPredictionService,
        ILogger<RelatorioService> logger)
    {
        _relatorioRepository = relatorioRepository;
        _userRepository = userRepository;
        _equipeRepository = equipeRepository;
        _reconhecimentoRepository = reconhecimentoRepository;
        _sentimentosRepository = sentimentosRepository;
        _mlPredictionService = mlPredictionService;
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

        // Obter dados da equipe para ML
        var equipe = await _equipeRepository.GetByIdAsync(usuario.IdEquipe.Value);
        var totalMembrosEquipe = equipe?.Users?.Count(u => u.Ativo == '1') ?? 1;
        var diasAtivos = 30;
        var taxaParticipacao = 100.0f;

        // Calcular taxa de participação (pessoas com sentimento registrado)
        if (equipe?.Users != null && equipe.Users.Any())
        {
            var usuariosComSentimento = 0;
            foreach (var membro in equipe.Users.Where(u => u.Ativo == '1'))
            {
                var temSentimento = await _sentimentosRepository.GetPontuacaoMediaAsync(membro.Id, dataInicio, dataFim);
                if (temSentimento.HasValue)
                    usuariosComSentimento++;
            }
            taxaParticipacao = totalMembrosEquipe > 0 ? (usuariosComSentimento * 100.0f) / totalMembrosEquipe : 0;
        }

        // Usar ML para prever engajamento pessoal
        var sentimentoParaML = pontuacaoMedia.HasValue ? (float)pontuacaoMedia.Value : 5.0f;
        var engajamentoPrevisto = _mlPredictionService.PreverEngajamentoEquipe(
            numeroMembros: totalMembrosEquipe,
            reconhecimentosMes: numeroIndicacoes,
            sentimentoMedio: sentimentoParaML,
            taxaParticipacao: taxaParticipacao,
            diasAtivos: diasAtivos
        );

        var classificacao = _mlPredictionService.ClassificarEngajamento(engajamentoPrevisto);
        var recomendacoes = _mlPredictionService.GerarRecomendacoes(engajamentoPrevisto, sentimentoParaML, numeroIndicacoes);

        var descritivo = $"Analise ML: {classificacao}. ";
        descritivo += $"Reconhecimentos: {numeroIndicacoes}. ";
        
        if (pontuacaoMedia.HasValue)
        {
            descritivo += $"Sentimento: {pontuacaoMedia.Value:F2}/10. ";
        }

        descritivo += $"Engajamento: {engajamentoPrevisto:F2}%. ";

        if (!string.IsNullOrEmpty(recomendacoes))
        {
            descritivo += $"Recomendacoes: {recomendacoes}";
        }

        // Limitar a 250 caracteres
        if (descritivo.Length > 250)
            descritivo = descritivo.Substring(0, 247) + "...";

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

        // Calcular reconhecimentos totais da equipe no mês
        var reconhecimentosMes = 0;
        var diasAtivos = 30;
        var usuariosComSentimento = 0;

        if (equipe.Users != null)
        {
            foreach (var usuario in equipe.Users.Where(u => u.Ativo == '1'))
            {
                var recsRecebidos = await _reconhecimentoRepository.GetTotalReconhecimentosRecebidosAsync(usuario.Id, dataInicio, dataFim);
                reconhecimentosMes += recsRecebidos;

                var temSentimento = await _sentimentosRepository.GetPontuacaoMediaAsync(usuario.Id, dataInicio, dataFim);
                if (temSentimento.HasValue)
                    usuariosComSentimento++;
            }
        }

        var taxaParticipacao = totalMembros > 0 ? (usuariosComSentimento * 100.0f) / totalMembros : 0;
        var sentimentoParaML = pontuacaoMedia.HasValue ? (float)pontuacaoMedia.Value : 5.0f;

        // Usar ML para prever engajamento da equipe
        var engajamentoPrevisto = _mlPredictionService.PreverEngajamentoEquipe(
            numeroMembros: totalMembros,
            reconhecimentosMes: reconhecimentosMes,
            sentimentoMedio: sentimentoParaML,
            taxaParticipacao: taxaParticipacao,
            diasAtivos: diasAtivos
        );

        var classificacao = _mlPredictionService.ClassificarEngajamento(engajamentoPrevisto);
        var recomendacoes = _mlPredictionService.GerarRecomendacoes(engajamentoPrevisto, sentimentoParaML, reconhecimentosMes);

        string sentimentoMedio;
        string descritivo;

        sentimentoMedio = pontuacaoMedia.HasValue 
            ? (pontuacaoMedia.Value >= 8 ? "Excelente" :
               pontuacaoMedia.Value >= 6 ? "Bom" :
               pontuacaoMedia.Value >= 4 ? "Regular" : "Critico")
            : "Sem dados";

        descritivo = $"Analise ML: {classificacao}. ";
        descritivo += $"Equipe: {totalMembros} membros. ";
        descritivo += $"Reconhecimentos: {reconhecimentosMes}. ";
        descritivo += $"Participacao: {taxaParticipacao:F1}%. ";

        if (pontuacaoMedia.HasValue)
        {
            descritivo += $"Sentimento: {pontuacaoMedia.Value:F2}/10. ";
        }

        descritivo += $"Engajamento: {engajamentoPrevisto:F2}%. ";

        if (!string.IsNullOrEmpty(recomendacoes))
        {
            descritivo += $"Recomendacoes: {recomendacoes}";
        }

        // Limitar a 250 caracteres
        if (descritivo.Length > 250)
            descritivo = descritivo.Substring(0, 247) + "...";

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
