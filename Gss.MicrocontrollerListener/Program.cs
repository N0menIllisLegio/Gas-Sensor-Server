using Gss.Infrastructure;
using Gss.MicrocontrollerListener;
using Gss.Queue;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    .ConfigureResource(c => c.AddService("Gss.MicrocontrollerListener"))
    .WithMetrics(metrics =>
    {
        metrics
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
            .AddSource(DiagnosticHeaders.DefaultListenerName)
            .AddHttpClientInstrumentation();
    });

var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

if (useOtlpExporter)
{
    builder.Services.AddOpenTelemetry().UseOtlpExporter();
}

builder.Services.Configure<MicrocontrollersConnectionsOptions>(
    builder.Configuration.GetSection(MicrocontrollersConnectionsOptions.SectionName));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"),
        npgsqlOptionsBuilder => npgsqlOptionsBuilder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)));

builder.Services.AddMassTransit(x => x.UsingRabbitMq((_, configurator) =>
{
    configurator.Host(builder.Configuration["RabbitMQ:HostName"], "/", hostConfigurator =>
    {
        hostConfigurator.Username(builder.Configuration["RabbitMQ:UserName"]!);
        hostConfigurator.Password(builder.Configuration["RabbitMQ:Password"]!);
    });

    configurator.ConfigureMessageTopology();
}));

builder.Services.AddHostedService<MicrocontrollerListener>();

var app = builder.Build();

app.Run();