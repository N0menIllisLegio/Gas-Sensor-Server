using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gss.Core.Helpers
{
  // TODO to Infrastructure
  public static class RolesManagerExtensions
  {
    public static async Task<List<IdentityRole<Guid>>> GetPage(this RoleManager<IdentityRole<Guid>> roleManager,
      int pageNumber, int pageSize, bool orderAsc, string filter)
    {
      var filteredRoles = roleManager.Roles
        .Where(r => r.Name.Contains(filter));

      var allRoles = orderAsc
        ? await filteredRoles.OrderBy(r => r.Name)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync()
        : await filteredRoles.OrderByDescending(r => r.Name)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

      return allRoles;
    }
  }
}
