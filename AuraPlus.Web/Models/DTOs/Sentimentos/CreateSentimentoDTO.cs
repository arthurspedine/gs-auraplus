using System.ComponentModel.DataAnnotations;

namespace AuraPlus.Web.Models.DTOs.Sentimentos;

public class CreateSentimentoDTO
{
    [Required(ErrorMessage = "Nome do sentimento é obrigatório")]
    [StringLength(50, ErrorMessage = "Nome do sentimento deve ter no máximo 50 caracteres")]
    public string NomeSentimento { get; set; } = string.Empty;

    [Range(0, 10, ErrorMessage = "Pontuação deve estar entre 0 e 10")]
    public decimal? ValorPontuacao { get; set; }

    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }
}
