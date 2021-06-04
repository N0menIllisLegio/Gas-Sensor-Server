using System;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Gss.Web.Configuration
{
  internal static class IdentityExtension
  {
    public static IdentityBuilder ConfigureIdentity(this IServiceCollection services)
    {
      return services.AddDefaultIdentity<User>(options =>
      {
        // TODO config for prod, max length set on front to 20
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
    }
  }
}
