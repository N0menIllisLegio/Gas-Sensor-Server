using System;
using System.Collections.Generic;
using System.Text;
using Gss.Core.Entities;
using Gss.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
      services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("LocalDB")));

      services.AddDefaultIdentity<User>(options =>
      {
        // TODO config for prod
        options.SignIn.RequireConfirmedAccount = true;
        options.SignIn.RequireConfirmedEmail = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 4;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.User.AllowedUserNameCharacters =
                  "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
        options.User.RequireUniqueEmail = true;
      })
          .AddRoles<IdentityRole<Guid>>()
          .AddEntityFrameworkStores<AppDbContext>()
          .AddUserManager<UserManager<User>>()
          .AddSignInManager<SignInManager<User>>()
          .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
          .AddDefaultTokenProviders();

      services.AddControllers();

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
              ValidateIssuer = true,
              ValidIssuer = Configuration["JwtIssuer"],
              ValidateAudience = true,
              ValidAudience = Configuration["JwtAudience"],
              ValidateLifetime = true,
              IssuerSigningKey = new SymmetricSecurityKey(
                          Encoding.ASCII.GetBytes(Configuration["JwtKey"])),
              ValidateIssuerSigningKey = true,
            };
          });

      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/build";
      });

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
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (!env.IsDevelopment())
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        // TODO app.UseHsts();
      }

      // TODO check app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller}/{action=Index}/{id?}");
      });

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
      });

      //app.UseSpa(spa =>
      //{
      //    spa.Options.SourcePath = "ClientApp";

      //    if (env.IsDevelopment())
      //    {
      //        spa.UseReactDevelopmentServer(npmScript: "start");
      //    }
      //});
    }
  }
}
