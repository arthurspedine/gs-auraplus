using Microsoft.ML.Data;

namespace AuraPlus.Trainer;

/// <summary>
/// Dados de entrada para treinamento de previsão de engajamento
/// </summary>
public class EngajamentoData
{
    [LoadColumn(0)]
    public float NumeroMembros { get; set; }

    [LoadColumn(1)]
    public float ReconhecimentosMes { get; set; }

    [LoadColumn(2)]
    public float SentimentoMedio { get; set; }

    [LoadColumn(3)]
    public float TaxaParticipacao { get; set; }

    [LoadColumn(4)]
    public float DiasAtivos { get; set; }

    /// <summary>
    /// Label: Nível de engajamento previsto (0-100)
    /// </summary>
    [LoadColumn(5)]
    public float NivelEngajamento { get; set; }
}

/// <summary>
/// Resultado da previsão
/// </summary>
public class EngajamentoPrediction
{
    [ColumnName("Score")]
    public float NivelEngajamentoPrevisto { get; set; }
}
