using System;
using System.Net;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Exceptions;
using Gss.Core.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Gss.Web.Middlewares
{
  public class ExceptionMiddleware
  {
    public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnvironment)
    {
      Next = next ?? throw new ArgumentNullException(nameof(next));
      Environment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
    }

    public IWebHostEnvironment Environment { get; }
    public RequestDelegate Next { get; }

    public async Task InvokeAsync(HttpContext context)
    {
      var body = context.Response.Body;

      try
      {
        await Next(context);

        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
          var response = new Response<object>().AddError(Messages.UnauthorizedMessageErrorString);

          await WriteResponseAsync(context, response, HttpStatusCode.Unauthorized);
        }
      }
      catch (Exception ex)
      {
        context.Response.Body = body;

        await HandleExceptionAsync(context, ex);
      }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
      string errorMessage = Messages.InternalServerErrorString;
      var statusCode = HttpStatusCode.InternalServerError;

      if (ex is AppException appException)
      {
        statusCode = appException.ErrorCode;
        errorMessage = ex.Message;
      }

      if (ex is FormatException)
      {
        statusCode = HttpStatusCode.BadRequest;
        errorMessage = ex.Message;
      }

      await WriteResponseAsync(context, new Response<object>().AddError(errorMessage), statusCode);
    }

    private async Task WriteResponseAsync(HttpContext context, object obj, HttpStatusCode statusCode)
    {
      var camelCaseFormatter = new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };

      camelCaseFormatter.Converters.Add(new StringEnumConverter(true));

      context.Response.Clear();
      context.Response.StatusCode = (int)statusCode;
      context.Response.ContentType = @"application/json";
      context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
      await context.Response.WriteAsync(JsonConvert.SerializeObject(obj, camelCaseFormatter));
    }
  }
}
