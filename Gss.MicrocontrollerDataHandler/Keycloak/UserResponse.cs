using System.Text.Json.Serialization;

namespace Gss.MicrocontrollerDataHandler.Keycloak;

internal record UserResponse
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }
}
