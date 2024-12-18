using Gss.Core.Interfaces.Repositories;
using Gss.Infrastructure;
using Gss.Infrastructure.Repositories;
using Gss.MicrocontrollerDataHandler.Consumers;
using Gss.MicrocontrollerDataHandler.Email;
using Gss.Queue;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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