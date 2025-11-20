using Microsoft.ML;
using AuraPlus.Web.ML;

namespace AuraPlus.Web.Services;

/// <summary>
/// Serviço para predições usando ML.NET
/// </summary>
public class MLPredictionService
{
    private readonly PredictionEngine<EngajamentoData, EngajamentoPrediction> _predictionEngine;
    private readonly ILogger<MLPredictionService> _logger;

    public MLPredictionService(ILogger<MLPredictionService> logger)
    {
        _logger = logger;

        try
        {
            var mlContext = new MLContext();
            var modelPath = "auraplus-ml-model.zip";

            // Carregar o modelo treinado
            var model = mlContext.Model.Load(modelPath, out var modelInputSchema);

            // Criar engine de predição
            _predictionEngine = mlContext.Model.CreatePredictionEngine<EngajamentoData, EngajamentoPrediction>(model);

            _logger.LogInformation("Modelo ML carregado com sucesso de {ModelPath}", modelPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar o modelo ML");
            throw;
        }
    }

    /// <summary>
    /// Prevê o nível de engajamento baseado em métricas de equipe
    /// </summary>
    public float PreverEngajamentoEquipe(
        int numeroMembros,
        int reconhecimentosMes,
        float sentimentoMedio,
        float taxaParticipacao,
        int diasAtivos)
    {
        var input = new EngajamentoData
        {
            NumeroMembros = numeroMembros,
            ReconhecimentosMes = reconhecimentosMes,
            SentimentoMedio = sentimentoMedio,
            TaxaParticipacao = taxaParticipacao,
            DiasAtivos = diasAtivos
        };

        var prediction = _predictionEngine.Predict(input);

        _logger.LogInformation(
            "Predição: Membros={Membros}, Reconhecimentos={Reconhecimentos}, Sentimento={Sentimento:F1}, " +
            "Participação={Participacao}%, Dias={Dias} => Engajamento Previsto={Engajamento:F2}%",
            numeroMembros, reconhecimentosMes, sentimentoMedio, taxaParticipacao, diasAtivos,
            prediction.NivelEngajamentoPrevisto);

        return prediction.NivelEngajamentoPrevisto;
    }

    /// <summary>
    /// Classifica o nível de engajamento em categorias
    /// </summary>
    public string ClassificarEngajamento(float nivelEngajamento)
    {
        return nivelEngajamento switch
        {
            >= 90 => "Excelente - Equipe altamente engajada!",
            >= 75 => "Bom - Equipe com engajamento saudável",
            >= 60 => "Moderado - Requer atenção para melhorias",
            >= 45 => "Baixo - Necessita intervenção urgente",
            _ => "Crítico - Situação requer ação imediata"
        };
    }

    /// <summary>
    /// Gera recomendações baseadas no nível de engajamento previsto
    /// </summary>
    public List<string> GerarRecomendacoes(float nivelEngajamento, float sentimentoMedio, int reconhecimentos)
    {
        var recomendacoes = new List<string>();

        if (nivelEngajamento < 60)
        {
            recomendacoes.Add("⚠️ Considere realizar atividades de team building");
        }

        if (sentimentoMedio < 6.0f)
        {
            recomendacoes.Add("😔 Sentimento da equipe está baixo - agende conversas individuais");
        }

        if (reconhecimentos < 10)
        {
            recomendacoes.Add("👏 Incentive mais reconhecimentos entre os membros");
        }

        if (nivelEngajamento >= 90)
        {
            recomendacoes.Add("🎉 Excelente trabalho! Continue mantendo este nível");
        }

        if (nivelEngajamento >= 75 && nivelEngajamento < 90)
        {
            recomendacoes.Add("✅ Bom engajamento - considere compartilhar as boas práticas");
        }

        return recomendacoes;
    }
}
