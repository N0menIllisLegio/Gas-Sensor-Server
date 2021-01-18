using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
using Gss.Core.Services;
using Gss.Infrastructure;
using Gss.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Gss.Web
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      LoadSettings();

      services.AddDbContext<AppDbContext>(options =>
          options.UseLazyLoadingProxies()
            .UseSqlServer(Configuration.GetConnectionString("LocalDB")));

      services.AddDefaultIdentity<User>(options =>
      {
        // TODO config for prod
        //options.SignIn.RequireConfirmedAccount = true;
        //options.SignIn.RequireConfirmedEmail = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 4;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.User.RequireUniqueEmail = true;
      })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddUserManager<UserManager>()
        .AddSignInManager<SignInManager<User>>()
        .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
        .AddDefaultTokenProviders();

      services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
          options.InvalidModelStateResponseFactory = actionContext =>
          {
            var errors = actionContext.ModelState.Values.SelectMany(v =>
              v.Errors.Select(b => b.ErrorMessage));

            return new BadRequestObjectResult(new Response<object>().AddErrors(errors));
          })
        .AddJsonOptions(options =>
        {
          options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
          options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.RequireHttpsMetadata = false;
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidIssuer = Settings.JWT.Issuer,
            ValidateAudience = true,
            ValidAudience = Settings.JWT.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = Settings.JWT.Key,
            ValidateIssuerSigningKey = true,
          };
        })
        .AddGoogle(options =>
        {
          var googleAuthNSection = Configuration.GetSection("Authentication:Google");

          options.ClientId = googleAuthNSection["ClientID"];
          options.ClientSecret = googleAuthNSection["ClientSecret"];
        });

      services.AddAutoMapper(typeof(AutoMapperProfile));
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

      services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/build");

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "GasSensor", Description = "ASP.NET Core project", Version = "v1" });

        c.AddSecurityDefinition("Bearer",
          new OpenApiSecurityScheme
          {
            In = ParameterLocation.Header,
            Description = "Please enter into field the word 'Bearer' following by space and JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
          });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,
            },
            new List<string>()
          }
        });

        c.EnableAnnotations();
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (!env.IsDevelopment())
      {
        // app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        // TODO app.UseHsts();
      }

      app.UseMiddleware<ExceptionMiddleware>();

      // TODO check app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints => endpoints.MapControllers());

      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

      //app.UseSpa(spa =>
      //{
      //    spa.Options.SourcePath = "ClientApp";

      //    if (env.IsDevelopment())
      //    {
      //        spa.UseReactDevelopmentServer(npmScript: "start");
      //    }
      //});
    }

    private void LoadSettings()
    {
      var jwtSection =
              Configuration.GetSection("Authentication:JWT");

      Settings.JWT.Issuer = jwtSection["Issuer"];
      Settings.JWT.Audience = jwtSection["Audience"];
      Settings.JWT.Key = new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes(jwtSection["Key"]));

      Settings.JWT.AccessTokenLifetimeMinutes = Int32.Parse(jwtSection["AccessTokenLifetimeMinutes"]);
      Settings.JWT.RefreshTokenLifetimeDays = Int32.Parse(jwtSection["RefreshTokenLifetimeDays"]);

      var emailSection = Configuration.GetSection("Email");
      string emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*"
        + @"@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

      Settings.Email.Address = emailSection["Address"];
      Settings.Email.Password = emailSection["Password"];
      Settings.Email.SmtpServer = emailSection["SmtpServer"];

      if (Int32.TryParse(emailSection["SmtpPort"], out int port)
        && Regex.IsMatch(Settings.Email.Address, emailRegex, RegexOptions.IgnoreCase))
      {
        Settings.Email.SmtpPort = port;
        Settings.Email.SmtpUseSsl = emailSection["SmtpUseSsl"].ToUpper() == "TRUE";
      }
      else
      {
        throw new ApplicationException(Messages.InvalidSettingsErrorString);
      }

      var azureImagesSection = Configuration.GetSection("AzureImages");

      Settings.AzureImages.AccountName = azureImagesSection["AccountName"];
      Settings.AzureImages.AccountKey = azureImagesSection["AccountKey"];
      Settings.AzureImages.ImagesContainer = azureImagesSection["ImagesContainer"];
      Settings.AzureImages.ThumbnailsContainer = azureImagesSection["ThumbnailsContainer"];
      Settings.AzureImages.SupportedExtensions = azureImagesSection.GetSection("SupportedExtensions").Get<List<string>>();
    }
  }
}
