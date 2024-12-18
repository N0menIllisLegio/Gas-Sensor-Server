using Gss.Infrastructure;
using Gss.MicrocontrollerListener.Data;
using Gss.MicrocontrollerListener.MicrocontrollerHandlers;
using Gss.Queue;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IListenerRepository, ListenerRepository>();

var app = builder.Build();

app.Run();