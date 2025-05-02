using StudManager.Application.DTOs;

namespace StudManager.Application.Interfaces;

public interface IQuoteService
{
    Task<QuoteDTO?> GetMotivationalQuoteAsync();
}