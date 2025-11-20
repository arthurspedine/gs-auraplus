using Microsoft.ML.Data;

namespace AuraPlus.Web.ML;

/// <summary>
/// Dados de entrada para previsão de engajamento
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
