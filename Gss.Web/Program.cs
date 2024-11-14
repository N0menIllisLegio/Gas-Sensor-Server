using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Services;
using Gss.Infrastructure;
using Gss.Web;
using Gss.Web.Configuration;
using Gss.Web.Middlewares;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

const string NotificationHubUrl = "/api/notifications";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureSettings();

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration.GetConnectionString("Database"),
            builder => builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null))
        .EnableSensitiveDataLogging());

builder.Services.ConfigureIdentity();
builder.Services.ConfigureControllers();
builder.Services.ConfigureAuthentication(builder.Configuration, NotificationHubUrl);
builder.Services.ConfigureSwagger();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddSignalR();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddTransient<ITokensService, TokensService>();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IMicrocontrollersService, MicrocontrollersService>();
builder.Services.AddScoped<ISensorsTypesService, SensorsTypesService>();
builder.Services.AddScoped<ISensorsService, SensorsService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<ISensorsDataService, SensorsDataService>();
builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();

builder.Services.AddSingleton<SocketConnectionService>();

builder.Services.AddSingleton<IUserIdProvider, UserEmailProvider>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseMiddleware<EnableRequestBufferingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationsHub>(NotificationHubUrl);
});

app.Services.GetRequiredService<SocketConnectionService>().RunAsync();

app.Run();