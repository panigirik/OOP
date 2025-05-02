using StudManager.Application.DTOs;
using StudManager.Application.Interfaces;

namespace StudManager.Application.Adapters;

public class QuoteAdapter : IQuoteService
{
    private readonly IExternalQuoteApi _api;
    public QuoteAdapter(IExternalQuoteApi api) => _api = api;

    public async Task<QuoteDTO?> GetMotivationalQuoteAsync()
    {
        var resp = await _api.FetchRandomQuoteAsync();
        return resp == null
            ? null
            : new QuoteDTO { Content = resp.Content, Author = resp.Author };
    }
}