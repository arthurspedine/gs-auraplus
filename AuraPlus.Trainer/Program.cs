using Microsoft.ML;
using AuraPlus.Trainer;

Console.WriteLine("=== AuraPlus ML Model Trainer ===");
Console.WriteLine();

// 1. Inicializar o Contexto do ML.NET
var mlContext = new MLContext(seed: 0);

// 2. Preparar dados de treinamento sintéticos
Console.WriteLine("Preparando dados de treinamento...");
var trainingData = new List<EngajamentoData>
{
    // Alta engajamento - Equipes grandes e ativas
    new EngajamentoData { NumeroMembros = 15, ReconhecimentosMes = 45, SentimentoMedio = 9.0f, TaxaParticipacao = 95, DiasAtivos = 28, NivelEngajamento = 95 },
    new EngajamentoData { NumeroMembros = 12, ReconhecimentosMes = 38, SentimentoMedio = 8.5f, TaxaParticipacao = 90, DiasAtivos = 26, NivelEngajamento = 92 },
    new EngajamentoData { NumeroMembros = 20, ReconhecimentosMes = 55, SentimentoMedio = 8.8f, TaxaParticipacao = 92, DiasAtivos = 27, NivelEngajamento = 94 },
    new EngajamentoData { NumeroMembros = 10, ReconhecimentosMes = 32, SentimentoMedio = 9.2f, TaxaParticipacao = 97, DiasAtivos = 29, NivelEngajamento = 96 },
    new EngajamentoData { NumeroMembros = 18, ReconhecimentosMes = 50, SentimentoMedio = 8.7f, TaxaParticipacao = 93, DiasAtivos = 28, NivelEngajamento = 93 },
    
    // Bom engajamento - Equipes médias com boa participação
    new EngajamentoData { NumeroMembros = 8, ReconhecimentosMes = 25, SentimentoMedio = 7.5f, TaxaParticipacao = 80, DiasAtivos = 22, NivelEngajamento = 82 },
    new EngajamentoData { NumeroMembros = 10, ReconhecimentosMes = 28, SentimentoMedio = 7.8f, TaxaParticipacao = 85, DiasAtivos = 24, NivelEngajamento = 85 },
    new EngajamentoData { NumeroMembros = 6, ReconhecimentosMes = 20, SentimentoMedio = 7.2f, TaxaParticipacao = 78, DiasAtivos = 20, NivelEngajamento = 80 },
    new EngajamentoData { NumeroMembros = 12, ReconhecimentosMes = 30, SentimentoMedio = 7.6f, TaxaParticipacao = 82, DiasAtivos = 23, NivelEngajamento = 83 },
    new EngajamentoData { NumeroMembros = 9, ReconhecimentosMes = 27, SentimentoMedio = 7.9f, TaxaParticipacao = 84, DiasAtivos = 25, NivelEngajamento = 84 },
    
    // Moderado engajamento - Equipes com participação irregular
    new EngajamentoData { NumeroMembros = 7, ReconhecimentosMes = 15, SentimentoMedio = 6.5f, TaxaParticipacao = 65, DiasAtivos = 18, NivelEngajamento = 68 },
    new EngajamentoData { NumeroMembros = 5, ReconhecimentosMes = 12, SentimentoMedio = 6.2f, TaxaParticipacao = 60, DiasAtivos = 16, NivelEngajamento = 65 },
    new EngajamentoData { NumeroMembros = 8, ReconhecimentosMes = 18, SentimentoMedio = 6.8f, TaxaParticipacao = 70, DiasAtivos = 19, NivelEngajamento = 70 },
    new EngajamentoData { NumeroMembros = 10, ReconhecimentosMes = 20, SentimentoMedio = 6.4f, TaxaParticipacao = 68, DiasAtivos = 17, NivelEngajamento = 67 },
    new EngajamentoData { NumeroMembros = 6, ReconhecimentosMes = 14, SentimentoMedio = 6.6f, TaxaParticipacao = 62, DiasAtivos = 18, NivelEngajamento = 66 },
    
    // Baixo engajamento - Equipes com pouca atividade
    new EngajamentoData { NumeroMembros = 5, ReconhecimentosMes = 8, SentimentoMedio = 5.0f, TaxaParticipacao = 45, DiasAtivos = 12, NivelEngajamento = 48 },
    new EngajamentoData { NumeroMembros = 4, ReconhecimentosMes = 6, SentimentoMedio = 4.8f, TaxaParticipacao = 40, DiasAtivos = 10, NivelEngajamento = 45 },
    new EngajamentoData { NumeroMembros = 6, ReconhecimentosMes = 10, SentimentoMedio = 5.2f, TaxaParticipacao = 50, DiasAtivos = 14, NivelEngajamento = 50 },
    new EngajamentoData { NumeroMembros = 3, ReconhecimentosMes = 5, SentimentoMedio = 4.5f, TaxaParticipacao = 38, DiasAtivos = 9, NivelEngajamento = 42 },
    new EngajamentoData { NumeroMembros = 7, ReconhecimentosMes = 11, SentimentoMedio = 5.5f, TaxaParticipacao = 48, DiasAtivos = 13, NivelEngajamento = 52 },
    
    // Crítico - Equipes com problemas sérios
    new EngajamentoData { NumeroMembros = 3, ReconhecimentosMes = 2, SentimentoMedio = 3.0f, TaxaParticipacao = 20, DiasAtivos = 5, NivelEngajamento = 25 },
    new EngajamentoData { NumeroMembros = 4, ReconhecimentosMes = 3, SentimentoMedio = 3.5f, TaxaParticipacao = 25, DiasAtivos = 6, NivelEngajamento = 28 },
    new EngajamentoData { NumeroMembros = 2, ReconhecimentosMes = 1, SentimentoMedio = 2.8f, TaxaParticipacao = 18, DiasAtivos = 4, NivelEngajamento = 22 },
    new EngajamentoData { NumeroMembros = 5, ReconhecimentosMes = 4, SentimentoMedio = 3.2f, TaxaParticipacao = 22, DiasAtivos = 7, NivelEngajamento = 26 },
    new EngajamentoData { NumeroMembros = 3, ReconhecimentosMes = 2, SentimentoMedio = 3.8f, TaxaParticipacao = 28, DiasAtivos = 8, NivelEngajamento = 30 },
    
    // Variações adicionais para melhor treinamento
    new EngajamentoData { NumeroMembros = 11, ReconhecimentosMes = 35, SentimentoMedio = 8.0f, TaxaParticipacao = 88, DiasAtivos = 25, NivelEngajamento = 88 },
    new EngajamentoData { NumeroMembros = 13, ReconhecimentosMes = 40, SentimentoMedio = 8.3f, TaxaParticipacao = 91, DiasAtivos = 27, NivelEngajamento = 91 },
    new EngajamentoData { NumeroMembros = 9, ReconhecimentosMes = 22, SentimentoMedio = 7.0f, TaxaParticipacao = 75, DiasAtivos = 21, NivelEngajamento = 76 },
    new EngajamentoData { NumeroMembros = 7, ReconhecimentosMes = 16, SentimentoMedio = 6.0f, TaxaParticipacao = 58, DiasAtivos = 15, NivelEngajamento = 62 },
    new EngajamentoData { NumeroMembros = 4, ReconhecimentosMes = 7, SentimentoMedio = 4.0f, TaxaParticipacao = 35, DiasAtivos = 8, NivelEngajamento = 38 },
};

Console.WriteLine($"Total de amostras de treinamento: {trainingData.Count}");

// 3. Carregar os dados
var dataView = mlContext.Data.LoadFromEnumerable(trainingData);

// 4. Definir o Pipeline de Treino
Console.WriteLine("Configurando pipeline de treinamento...");
var pipeline = mlContext.Transforms.Concatenate("Features",
        nameof(EngajamentoData.NumeroMembros),
        nameof(EngajamentoData.ReconhecimentosMes),
        nameof(EngajamentoData.SentimentoMedio),
        nameof(EngajamentoData.TaxaParticipacao),
        nameof(EngajamentoData.DiasAtivos))
    .Append(mlContext.Regression.Trainers.FastTree(
        labelColumnName: nameof(EngajamentoData.NivelEngajamento),
        numberOfLeaves: 20,
        numberOfTrees: 100,
        minimumExampleCountPerLeaf: 2));

// 5. Treinar o modelo
Console.WriteLine("Treinando o modelo...");
var model = pipeline.Fit(dataView);
Console.WriteLine("✓ Modelo treinado com sucesso!");

// 6. Salvar o modelo em um ficheiro .zip
var modelPath = "auraplus-ml-model.zip";
mlContext.Model.Save(model, dataView.Schema, modelPath);

Console.WriteLine();
Console.WriteLine($"✓ Modelo salvo como '{modelPath}'");
Console.WriteLine();
Console.WriteLine("PRÓXIMOS PASSOS:");
Console.WriteLine($"1. Copie o arquivo '{modelPath}' para a raiz do projeto AuraPlus.Web");
Console.WriteLine("2. O MLPredictionService irá carregar automaticamente este modelo");
Console.WriteLine();

// 7. Fazer previsões de teste
Console.WriteLine("=== Testes de Previsão ===");
Console.WriteLine();

var predictionEngine = mlContext.Model.CreatePredictionEngine<EngajamentoData, EngajamentoPrediction>(model);

var testCases = new[]
{
    new EngajamentoData { NumeroMembros = 15, ReconhecimentosMes = 42, SentimentoMedio = 8.5f, TaxaParticipacao = 90, DiasAtivos = 27 },
    new EngajamentoData { NumeroMembros = 8, ReconhecimentosMes = 18, SentimentoMedio = 6.5f, TaxaParticipacao = 65, DiasAtivos = 18 },
    new EngajamentoData { NumeroMembros = 3, ReconhecimentosMes = 3, SentimentoMedio = 3.5f, TaxaParticipacao = 25, DiasAtivos = 6 }
};

string[] scenarios = { "Equipe de Alto Desempenho", "Equipe Moderada", "Equipe Crítica" };

for (int i = 0; i < testCases.Length; i++)
{
    var testData = testCases[i];
    var prediction = predictionEngine.Predict(testData);
    
    Console.WriteLine($"📊 {scenarios[i]}:");
    Console.WriteLine($"   Membros: {testData.NumeroMembros}");
    Console.WriteLine($"   Reconhecimentos/Mês: {testData.ReconhecimentosMes}");
    Console.WriteLine($"   Sentimento Médio: {testData.SentimentoMedio:F1}/10");
    Console.WriteLine($"   Taxa Participação: {testData.TaxaParticipacao}%");
    Console.WriteLine($"   Dias Ativos: {testData.DiasAtivos}");
    Console.WriteLine($"   ➜ Nível de Engajamento Previsto: {prediction.NivelEngajamentoPrevisto:F2}%");
    Console.WriteLine();
}

Console.WriteLine("Treinamento concluído com sucesso!");
