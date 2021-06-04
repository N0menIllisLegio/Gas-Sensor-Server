using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Gss.Web.Configuration
{
  internal static class SwaggerExtension
  {
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
      return services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "GasSensor", Description = "ASP.NET Core project", Version = "v1" });

        c.AddSecurityDefinition("Bearer",
          new OpenApiSecurityScheme
          {
            In = ParameterLocation.Header,
            Description = "Please enter into field the word 'Bearer' following by space and JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
          });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,
            },
            new List<string>()
          }
        });

        c.EnableAnnotations();
      });
    }
  }
}
