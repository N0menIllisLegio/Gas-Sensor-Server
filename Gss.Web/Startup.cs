using System;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Services;
using Gss.Infrastructure;
using Gss.Web.Configuration;
using Gss.Web.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gss.Web
{
  public class Startup
  {
    public const string NotificationHubUrl = "/api/notifications";
    public const string ReactDevelopmentServerUrl = "http://localhost:3000";

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      Configuration.ConfigureSettings();

      services.AddDbContext<AppDbContext>(options =>
        options.UseLazyLoadingProxies()
          .UseSqlServer(Configuration.GetConnectionString("AzureDB"),
            builder => builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null))
          .EnableSensitiveDataLogging());

      services.ConfigureIdentity();
      services.ConfigureControllers();
      services.ConfigureAuthentication(Configuration, NotificationHubUrl);
      services.ConfigureSwagger();

      services.AddAutoMapper(typeof(AutoMapperProfile));
      services.AddSignalR();
      services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/build");

      services.AddScoped<IUnitOfWork, UnitOfWork>();

      services.AddTransient<ITokensService, TokensService>();
      services.AddTransient<IEmailService, EmailService>();

      services.AddScoped<IAuthenticationService, AuthenticationService>();
      services.AddScoped<IMicrocontrollersService, MicrocontrollersService>();
      services.AddScoped<ISensorsTypesService, SensorsTypesService>();
      services.AddScoped<ISensorsService, SensorsService>();
      services.AddScoped<IRolesService, RolesService>();
      services.AddScoped<IFilesService, FilesService>();
      services.AddScoped<IUsersService, UsersService>();
      services.AddScoped<ISensorsDataService, SensorsDataService>();

      services.AddSingleton<SocketConnectionService>();

      services.AddSingleton<IUserIdProvider, UserEmailProvider>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SocketConnectionService socketConnectionService)
    {
      if (!env.IsDevelopment())
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        // TODO app.UseHsts();
      }

      app.UseMiddleware<EnableRequestBufferingMiddleware>();
      app.UseMiddleware<ExceptionMiddleware>();

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseRouting();

      app.UseCors(builder => builder
        .WithOrigins(ReactDevelopmentServerUrl)
        .AllowAnyHeader().AllowAnyMethod()
        .AllowCredentials());

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints => endpoints.MapControllers());

      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

      socketConnectionService.RunAsync();

      app.UseEndpoints(endpoints => endpoints.MapHub<NotificationsHub>(NotificationHubUrl));

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          spa.UseProxyToSpaDevelopmentServer(ReactDevelopmentServerUrl);
        }
      });
    }
  }
}
