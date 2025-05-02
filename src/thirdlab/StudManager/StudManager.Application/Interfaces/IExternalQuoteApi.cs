using StudManager.Application.Responses;

namespace StudManager.Application.Interfaces;

public interface IExternalQuoteApi
{
    Task<QuoteApiResponse?> FetchRandomQuoteAsync();
}