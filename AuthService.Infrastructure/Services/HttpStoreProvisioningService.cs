using AuthService.Application.Dtos.Integration;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Results;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AuthService.Infrastructure.Services;

public sealed class HttpStoreProvisioningService : IStoreProvisioningService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpStoreProvisioningService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public Task<ServiceResult> ProvisionStoreAsync(int tenantId, string slug)
    {
        return SendAsync(
            tenantId,
            new
            {
                name = slug
            },
            _configuration["ECommerce:ProvisionStorePathTemplate"] ?? "/api/admin/stores/{0}/provision");
    }

    public Task<ServiceResult> ProvisionCustomerAsync(int tenantId, Guid externalUserId, string email, string firstName, string lastName)
    {
        var request = new SyncCustomerRequest(externalUserId, email, firstName, lastName);
        return SendAsync(
            tenantId,
            request,
            _configuration["ECommerce:SyncCustomerPathTemplate"] ?? "/api/internal/customers/{0}/sync");
    }

    private async Task<ServiceResult> SendAsync(int tenantId, object payload, string pathTemplate)
    {
        var baseUrl = _configuration["ECommerce:BaseUrl"];
        var serviceToken = _configuration["ServiceTokens:ECommerce"];

        if (string.IsNullOrWhiteSpace(baseUrl))
            return ServiceResult.Fail("ECommerce base url is not configured.");

        if (string.IsNullOrWhiteSpace(serviceToken))
            return ServiceResult.Fail("ECommerce service token is not configured.");

        var path = string.Format(pathTemplate, tenantId);
        var requestUri = new Uri(new Uri(baseUrl.TrimEnd('/')), path.TrimStart('/'));

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceToken);

        using var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
            return ServiceResult.Ok("Provisioning request sent successfully.");

        var errorBody = await response.Content.ReadAsStringAsync();
        var message = string.IsNullOrWhiteSpace(errorBody)
            ? $"ECommerce call failed with status code {(int)response.StatusCode}."
            : $"ECommerce call failed with status code {(int)response.StatusCode}: {errorBody}";

        return ServiceResult.Fail(message);
    }
}
