using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Gss.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gss.MicrocontrollerDataHandler.Keycloak;

internal sealed class KeycloakHttpClient
{
    public static readonly ActivitySource ActivitySource =
        new("Gss.MicrocontrollerDataHandler.Keycloak.KeycloakHttpClient", "1.0.0");

    private readonly HttpClient _httpClient;
    private readonly ILogger<KeycloakHttpClient> _logger;
    private readonly KeycloakConfiguration _options;

    private static AuthResponse? _authResponse;

    public KeycloakHttpClient(HttpClient httpClient, IOptions<KeycloakConfiguration> options,
        ILogger<KeycloakHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
    }

    public async Task<UserResponse> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        using Activity? activity = ActivitySource.StartActivity();
        activity?.SetTag("UserId", userId);

        if (_authResponse == null || _authResponse.ExpiresIn < DateTime.Now)
            await GetAccessTokenAsync(cancellationToken);

        var response = await _httpClient.SendAsync(new HttpRequestMessage()
        {
            RequestUri = new Uri($"admin/realms/{_options.Realm}/users/{userId}", UriKind.Relative),
            Method = HttpMethod.Get,
            Headers =
            {
                Authorization = new AuthenticationHeaderValue(_authResponse!.TokenType, _authResponse.AccessToken)
            }
        }, cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var responseContent = await response.Content.ReadAsStreamAsync(cancellationToken);

        var user = await JsonSerializer.DeserializeAsync<UserResponse>(
            responseContent, cancellationToken: cancellationToken);

        if (user == null)
        {
            _logger.LogError("Failed to deserialize KeyCloak's user ({UserId}) response", userId);

            activity?.SetStatus(ActivityStatusCode.Error);
        }
        else
        {
            activity?.SetStatus(ActivityStatusCode.Ok);
        }

        return user ?? throw new AppException($"Failed to deserialize KeyCloak user ({userId}) response");
    }

    private async Task GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        using Activity? activity = ActivitySource.StartActivity();

        using var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _options.ClientId),
            new KeyValuePair<string, string>("client_secret", _options.ClientSecret)
        });

        var response = await _httpClient.SendAsync(new HttpRequestMessage()
        {
            RequestUri = new Uri($"realms/{_options.Realm}/protocol/openid-connect/token", UriKind.Relative),
            Method = HttpMethod.Post,
            Content = formContent
        }, cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var responseContent = await response.Content.ReadAsStreamAsync(cancellationToken);

        var authResponse = await JsonSerializer.DeserializeAsync<AuthResponse>(
            responseContent, cancellationToken: cancellationToken);

        if (authResponse == null)
        {
            _logger.LogWarning("Failed to deserialize KeyCloak's auth response");

            activity?.SetStatus(ActivityStatusCode.Error);
        }
        else
        {
            activity?.SetStatus(ActivityStatusCode.Ok);
        }

        _authResponse = authResponse ?? throw new AppException("Failed to deserialize KeyCloak's auth response");
    }
}
