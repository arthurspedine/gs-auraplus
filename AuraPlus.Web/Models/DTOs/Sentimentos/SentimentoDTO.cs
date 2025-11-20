namespace AuraPlus.Web.Models.DTOs.Sentimentos;

public class SentimentoDTO
{
    public int Id { get; set; }
    public string NomeSentimento { get; set; } = string.Empty;
    public decimal? ValorPontuacao { get; set; }
    public DateTime Data { get; set; }
    public string? Descricao { get; set; }
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;

    public static SentimentoDTO FromSentimento(Models.Sentimentos sentimento)
    {
        return new SentimentoDTO
        {
            Id = sentimento.Id,
            NomeSentimento = sentimento.NomeSentimento,
            ValorPontuacao = sentimento.ValorPontuacao,
            Data = sentimento.Data,
            Descricao = sentimento.Descricao,
            IdUsuario = sentimento.IdUsuario,
            NomeUsuario = sentimento.Usuario?.Nome ?? string.Empty
        };
    }
}
