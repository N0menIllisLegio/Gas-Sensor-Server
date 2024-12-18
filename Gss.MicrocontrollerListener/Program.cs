using Gss.Infrastructure;
using Gss.MicrocontrollerListener;
using Gss.Queue;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

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