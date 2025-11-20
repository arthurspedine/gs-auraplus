namespace AuraPlus.Test;

/// <summary>
/// Testes unitários para a lógica de classificação de engajamento
/// </summary>
public class MLPredictionServiceTests
{
    // Simula a lógica de classificação do MLPredictionService
    private string ClassificarEngajamento(float nivelEngajamento)
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

    [Fact]
    public void ClassificarEngajamento_QuandoNivelExcelente_DeveRetornarMensagemCorreta()
    {
        // Arrange
        var nivelEngajamento = 95.0f;
        var esperado = "Excelente - Equipe altamente engajada!";

        // Act
        var resultado = ClassificarEngajamento(nivelEngajamento);

        // Assert
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void ClassificarEngajamento_QuandoNivelBom_DeveRetornarMensagemCorreta()
    {
        // Arrange
        var nivelEngajamento = 80.0f;
        var esperado = "Bom - Equipe com engajamento saudável";

        // Act
        var resultado = ClassificarEngajamento(nivelEngajamento);

        // Assert
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void ClassificarEngajamento_QuandoNivelModerado_DeveRetornarMensagemCorreta()
    {
        // Arrange
        var nivelEngajamento = 65.0f;
        var esperado = "Moderado - Requer atenção para melhorias";

        // Act
        var resultado = ClassificarEngajamento(nivelEngajamento);

        // Assert
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void ClassificarEngajamento_QuandoNivelBaixo_DeveRetornarMensagemCorreta()
    {
        // Arrange
        var nivelEngajamento = 50.0f;
        var esperado = "Baixo - Necessita intervenção urgente";

        // Act
        var resultado = ClassificarEngajamento(nivelEngajamento);

        // Assert
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void ClassificarEngajamento_QuandoNivelCritico_DeveRetornarMensagemCorreta()
    {
        // Arrange
        var nivelEngajamento = 20.0f;
        var esperado = "Crítico - Situação requer ação imediata";

        // Act
        var resultado = ClassificarEngajamento(nivelEngajamento);

        // Assert
        Assert.Equal(esperado, resultado);
    }
}
