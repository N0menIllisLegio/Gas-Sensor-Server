namespace Gss.MicrocontrollerDataHandler.Keycloak;

internal record KeycloakConfiguration
{
    public required string BaseUrl { get; init; }
    public required string Realm { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}
