using System.Net.Http.Json;
using StudManager.Application.Interfaces;
using StudManager.Application.Responses;

namespace StudManager.Application.Services
{
    public class ExternalQuoteApi : IExternalQuoteApi
    {
        private readonly HttpClient _httpClient;

        // Внедряем HttpClient через конструктор
        public ExternalQuoteApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<QuoteApiResponse?> FetchRandomQuoteAsync() =>
            _httpClient.GetFromJsonAsync<QuoteApiResponse>("https://api.quotable.io/random");
    }
}