using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Gss.Core.Helpers
{
  public static class RoleMangerExtensions
  {
    public static Task<IdentityRole<Guid>> FindByIdAsync(this RoleManager<IdentityRole<Guid>> roleManager, Guid id)
    {
      return roleManager.FindByIdAsync(id.ToString());
    }
  }
}
