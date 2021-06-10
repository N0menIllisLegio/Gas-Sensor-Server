using System;
using System.Threading.Tasks;
using Gss.Core.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Gss.Web.Configuration
{
  internal static class AuthenticationExtension
  {
    public static AuthenticationBuilder ConfigureAuthentication(this IServiceCollection services,
      IConfiguration configuration, string notificationHubUrl)
    {
      return services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options =>
      {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = Settings.JWT.Issuer,
          ValidateAudience = true,
          ValidAudience = Settings.JWT.Audience,
          ValidateLifetime = true,
          IssuerSigningKey = Settings.JWT.Key,
          ValidateIssuerSigningKey = true,
        };

        options.Events = new JwtBearerEvents
        {
          OnMessageReceived = context =>
          {
            string accessToken = context.Request.Query["access_token"];

            if (!String.IsNullOrEmpty(accessToken)
              && context.HttpContext.Request.Path.StartsWithSegments(notificationHubUrl))
            {
              context.Token = accessToken;
            }

            return Task.CompletedTask;
          }
        };
      })
      .AddGoogle(options =>
      {
        var googleAuthNSection = configuration.GetSection("Authentication:Google");

        options.ClientId = googleAuthNSection["ClientID"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];
      });
    }
  }
}
