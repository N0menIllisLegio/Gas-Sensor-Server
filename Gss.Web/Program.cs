using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Gss.Core.Exceptions;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Models;
using Gss.Core.Services;
using Gss.Infrastructure;
using Gss.MicrocontrollerDataReceiver;
using Gss.Web;
using Gss.Web.Configuration;
using Gss.Web.CurrentUser;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

const string NotificationHubUrl = "/api/notifications";

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection(EmailOptions.SectionName));

builder.Services.Configure<MicrocontrollersConnectionsOptions>(
    builder.Configuration.GetSection(MicrocontrollersConnectionsOptions.SectionName));

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration.GetConnectionString("Database"),
            npgsqlOptionsBuilder => npgsqlOptionsBuilder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)));

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

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
    })
    .AddProblemDetailsConventions();

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

        // TODO:
        // o.Events = new JwtBearerEvents
        // {
        //     OnMessageReceived = context =>
        //     {
        //         string accessToken = context.Request.Query["access_token"];
        //
        //         if (!String.IsNullOrEmpty(accessToken)
        //             && context.HttpContext.Request.Path.StartsWithSegments(NotificationHubUrl))
        //         {
        //             context.Token = accessToken;
        //         }
        //
        //         return Task.CompletedTask;
        //     }
        // };
    });

builder.Services.ConfigureSwagger(builder.Configuration);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddSignalR();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IMicrocontrollersService, MicrocontrollersService>();
builder.Services.AddScoped<ISensorsTypesService, SensorsTypesService>();
builder.Services.AddScoped<ISensorsService, SensorsService>();
builder.Services.AddScoped<ISensorsDataService, SensorsDataService>();

builder.Services.AddSingleton<SocketConnectionService>();

builder.Services.AddSingleton<IUserIdProvider, UserEmailProvider>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<ICurrentUserDataSetter, CurrentUser>(
    c => (CurrentUser)c.GetRequiredService<ICurrentUser>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseProblemDetails();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CurrentUserDataSetterMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationsHub>(NotificationHubUrl);
});

app.Services.GetRequiredService<SocketConnectionService>().RunAsync();

app.Run();