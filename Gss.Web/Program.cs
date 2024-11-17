using FluentValidation;
using FluentValidation.AspNetCore;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Services;
using Gss.Infrastructure;
using Gss.Web;
using Gss.Web.Configuration;
using Gss.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

const string NotificationHubUrl = "/api/notifications";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureSettings();

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration.GetConnectionString("Database"),
            npgsqlOptionsBuilder => npgsqlOptionsBuilder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)));

builder.Services.ConfigureControllers();
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

// TODO: replace with problem details
app.UseMiddleware<ExceptionMiddleware>();

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