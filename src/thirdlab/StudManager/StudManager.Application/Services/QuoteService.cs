using System.Net.Http.Json;
using StudManager.Application.DTOs;
using StudManager.Application.Interfaces;
using StudManager.Application.Responses;

namespace StudManager.Application.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly HttpClient _httpClient;

        public QuoteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<QuoteDTO?> GetMotivationalQuoteAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<QuoteApiResponse>("https://api.quotable.io/random");
                
                if (response == null || string.IsNullOrWhiteSpace(response.Content))
                    return null;
                
                return new QuoteDTO
                {
                    Content = response.Content,
                    Author = response.Author
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}