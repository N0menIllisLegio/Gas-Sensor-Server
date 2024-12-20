using Gss.Core.Interfaces.Repositories;
using Gss.Infrastructure;
using Gss.Infrastructure.Repositories;
using Gss.MicrocontrollerDataHandler.Consumers;
using Gss.MicrocontrollerDataHandler.Email;
using Gss.Queue;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(c => c.AddService("Gss.MicrocontrollerDataHandler"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeter("Npgsql")
            .AddMeter(InstrumentationOptions.MeterName)
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation();
    })
    .WithTracing(tracing =>
    {
        if (builder.Environment.IsDevelopment())
        {
            tracing.SetSampler<AlwaysOnSampler>();
        }

        tracing
            .AddNpgsql()
            .AddSource(DiagnosticHeaders.DefaultListenerName)
            .AddSource(SensorDataReceivedConsumer.ActivitySource.Name)
            .AddSource(CriticalValueReachedConsumer.ActivitySource.Name)
            .AddHttpClientInstrumentation();
    });

var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

if (useOtlpExporter)
{
    builder.Services.AddOpenTelemetry().UseOtlpExporter();
}

builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection(EmailOptions.SectionName));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"),
        npgsqlOptionsBuilder => npgsqlOptionsBuilder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SensorDataReceivedConsumer, SensorDataReceivedConsumerDefinition>();
    x.AddConsumer<CriticalValueReachedConsumer, CriticalValueReachedConsumerDefinition>();

    x.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ:HostName"], "/", hostConfigurator =>
        {
            hostConfigurator.Username(builder.Configuration["RabbitMQ:UserName"]!);
            hostConfigurator.Password(builder.Configuration["RabbitMQ:Password"]!);
        });

        configurator.ConfigureMessageTopology();
        configurator.ConfigureEndpoints(context);
    });
});

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IMicrocontrollersRepository, MicrocontrollersRepository>();

var app = builder.Build();

app.Run();