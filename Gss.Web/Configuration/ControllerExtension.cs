using System.Text.Json.Serialization;
using Gss.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Gss.Web.Configuration;

internal static class ControllerExtension
{
  public static IMvcBuilder ConfigureControllers(this IServiceCollection services)
  {
    return services.AddControllers()
      .ConfigureApiBehaviorOptions(options =>
        options.InvalidModelStateResponseFactory = actionContext =>
        {
          // TODO: Problem details.
          var errors = actionContext.ModelState.Values.SelectMany(v =>
            v.Errors.Select(b => b.ErrorMessage));

          return new BadRequestObjectResult(new Response<object>().AddErrors(errors));
        })
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
      });
  }
}