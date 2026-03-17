using AuthService.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace AuthService.Infrastructure.Services
{
    public class HttpStoreProvisioningService : IStoreProvisioningService
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceToken;

        public HttpStoreProvisioningService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _serviceToken = config["ServiceTokens:ECommerce"]!;
        }

        public async Task ProvisionStoreAsync(int tenantId,string slug)
        {
            // Authorization header olarak service token ekle
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _serviceToken);

            var request = new 
            {
                Name = slug
            };

            var url = $"https://localhost:7177/api/admin/stores/{tenantId}/provision";
            var response = await _httpClient.PostAsJsonAsync(url, request);
            response.EnsureSuccessStatusCode();
        }
    }
}
