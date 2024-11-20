using Gss.Infrastructure;
using Gss.MicrocontrollerListener.Data;
using Gss.MicrocontrollerListener.Email;
using Gss.MicrocontrollerListener.MicrocontrollerHandlers;
using Gss.MicrocontrollerListener.Notifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection(EmailOptions.SectionName));

builder.Services.Configure<MicrocontrollersConnectionsOptions>(
    builder.Configuration.GetSection(MicrocontrollersConnectionsOptions.SectionName));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"),
        npgsqlOptionsBuilder => npgsqlOptionsBuilder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.Audience = builder.Configuration["Authentication:Audience"];
        o.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
        o.TokenValidationParameters = new()
        {
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ClockSkew = TimeSpan.Zero,
        };

        o.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                string? accessToken = context.Request.Query["access_token"];

                if (!string.IsNullOrEmpty(accessToken)
                    && context.HttpContext.Request.Path.StartsWithSegments(NotificationsHub.Url))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

builder.Services.AddTransient<IEmailService, EmailService>();
// builder.Services.AddSingleton<SocketConnectionService>();
builder.Services.AddHostedService<MicrocontrollerListener>();
builder.Services.AddScoped<IMicrocontrollerRequestsHandler, MicrocontrollerRequestsHandler>();

builder.Services.AddScoped<IListenerRepository, ListenerRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationsHub>(NotificationsHub.Url);

app.Run();