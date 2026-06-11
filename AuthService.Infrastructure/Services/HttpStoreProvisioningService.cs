using AuthService.Application.Dtos.Integration;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Results;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

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

    public Task<ServiceResult<StoreProvisioningResponse>> ProvisionStoreAsync(int tenantId, string name)
    {
        return ProvisionStoreAsync(tenantId, name, "starter");
    }

    public Task<ServiceResult<StoreProvisioningResponse>> ProvisionStoreAsync(int tenantId, string name, string planCode)
    {
        return SendAsync(
            tenantId,
            new
            {
                name,
                planCode
            },
            _configuration["ECommerce:ProvisionStorePathTemplate"] ?? "/api/admin/stores/{0}/provision");
    }

    public Task<ServiceResult> ProvisionCustomerAsync(int tenantId, Guid externalUserId, string email, string firstName, string lastName)
    {
        var request = new SyncCustomerRequest(externalUserId, email, firstName, lastName);
        return ProvisionCustomerInternalAsync(
            tenantId,
            request,
            _configuration["ECommerce:SyncCustomerPathTemplate"] ?? "/api/internal/customers/{0}/sync");
    }

    private async Task<ServiceResult<StoreProvisioningResponse>> SendAsync(int tenantId, object payload, string pathTemplate)
    {
        var baseUrl = GetFirstConfiguredValue("ECommerce:ApiBaseUrl", "ECOMMERCE_API_BASE_URL", "ECommerce:BaseUrl");
        var serviceToken = GetFirstConfiguredValue("ECommerce:ServiceToken", "ECOMMERCE_SERVICE_TOKEN", "ServiceTokens:ECommerce");

        if (string.IsNullOrWhiteSpace(baseUrl))
            return ServiceResult<StoreProvisioningResponse>.Fail("ECommerce base url is not configured.");

        if (string.IsNullOrWhiteSpace(serviceToken))
            return ServiceResult<StoreProvisioningResponse>.Fail("ECommerce service token is not configured.");

        var path = string.Format(pathTemplate, tenantId);
        var requestUri = new Uri(new Uri(baseUrl.TrimEnd('/')), path.TrimStart('/'));

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceToken);

        using var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            StoreProvisioningResponse? storeResponse = null;
            var responseBody = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(responseBody))
            {
                try
                {
                    storeResponse = JsonSerializer.Deserialize<StoreProvisioningResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch
                {
                    storeResponse = null;
                }
            }

            return ServiceResult<StoreProvisioningResponse>.Ok(storeResponse ?? new StoreProvisioningResponse(), "Provisioning request sent successfully.");
        }

        var errorBody = await response.Content.ReadAsStringAsync();
        var message = string.IsNullOrWhiteSpace(errorBody)
            ? $"ECommerce call failed with status code {(int)response.StatusCode}."
            : $"ECommerce call failed with status code {(int)response.StatusCode}: {errorBody}";

        return ServiceResult<StoreProvisioningResponse>.Fail(message);
    }

    private async Task<ServiceResult> ProvisionCustomerInternalAsync(int tenantId, SyncCustomerRequest payload, string pathTemplate)
    {
        var result = await SendAsync(tenantId, payload, pathTemplate);
        return result.Success
            ? ServiceResult.Ok(result.Message ?? "Provisioning request sent successfully.")
            : ServiceResult.Fail(result.Message ?? "ECommerce call failed.");
    }

    private string? GetFirstConfiguredValue(params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = _configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }
}
