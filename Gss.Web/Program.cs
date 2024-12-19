using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Gss.Core.Exceptions;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Services;
using Gss.Infrastructure;
using Gss.Infrastructure.Extensions;
using Gss.Web.Configuration;
using Gss.Web.CurrentUser;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(c => c.AddService("Gss.Web"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation();
    })
    .WithTracing(tracing =>
    {
        if (builder.Environment.IsDevelopment())
        {
            tracing.SetSampler<AlwaysOnSampler>();
        }

        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    });

var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

if (useOtlpExporter)
{
    builder.Services.AddOpenTelemetry().UseOtlpExporter();
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("Database"),
            npgsqlOptionsBuilder => npgsqlOptionsBuilder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)));

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services
    .AddProblemDetails(options =>
    {
        options.Map<UserInputException>(ex =>
        {
            var problemDetails = StatusCodeProblemDetails.Create(StatusCodes.Status400BadRequest);
            problemDetails.Detail = ex.Message;

            return problemDetails;
        });

        options.Map<NotFoundException>(ex =>
        {
            var problemDetails = StatusCodeProblemDetails.Create(StatusCodes.Status404NotFound);
            problemDetails.Detail = ex.Message;

            return problemDetails;
        });

        options.MapToStatusCode<OperationCanceledException>(StatusCodes.Status499ClientClosedRequest);
    })
    .AddProblemDetailsConventions();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.Audience = builder.Configuration["Authentication:Audience"];
        o.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.ConfigureSwagger(builder.Configuration);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMicrocontrollersService, MicrocontrollersService>();
builder.Services.AddScoped<ISensorsTypesService, SensorsTypesService>();
builder.Services.AddScoped<ISensorsService, SensorsService>();
builder.Services.AddScoped<ISensorsDataService, SensorsDataService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<ICurrentUserDataSetter, CurrentUser>(
    c => (CurrentUser)c.GetRequiredService<ICurrentUser>());

var app = builder.Build();

await app.ApplyMigrationsAsync();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseProblemDetails();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o => o.OAuthClientId("public-client"));
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CurrentUserDataSetterMiddleware>();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();