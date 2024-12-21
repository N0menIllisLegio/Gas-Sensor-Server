using System.Text.Json.Serialization;

namespace Gss.MicrocontrollerDataHandler.Keycloak;

internal record UserResponse
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("emailVerified")]
    public required bool EmailVerified { get; init; }
}
