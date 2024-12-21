using System.Text.Json.Serialization;

namespace Gss.MicrocontrollerDataHandler.Keycloak;

internal record AuthResponse
{
    private readonly int _expiresInTotalSeconds;

    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("expires_in")]
    public required int ExpiresInTotalSeconds
    {
        get => _expiresInTotalSeconds;
        init
        {
            ExpiresIn = DateTime.Now.AddSeconds(value);
            _expiresInTotalSeconds = value;
        }
    }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }

    [JsonIgnore]
    public DateTime ExpiresIn { get; private set; }
}
