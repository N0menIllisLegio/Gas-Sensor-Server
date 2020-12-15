using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gss.Core.Helpers
{
  // TODO Move to Infrastructure
  public class UserManager: UserManager<User>
  {
    private const string StandardRoleName = "User";

    public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
      IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
      IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
      IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger)
      : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer,
          errors, services, logger)
    { }

    public override async Task<IdentityResult> CreateAsync(User user)
    {
      user.UserName = user.Email;
      var result = await base.CreateAsync(user);

      if (!result.Succeeded)
      {
        return result;
      }

      return await AddToRoleAsync(user, StandardRoleName);
    }

    public async Task<List<User>> GetPage(int pageSize, int pageNumber,
      SortOrder sortOrder, string sortBy,
      string filterBy = null, string filterStr = null)
    {
      var filter = filterBy switch
      {
        "EMAIL" => (Expression<Func<User, bool>>)((user) => user.Email.Contains(filterStr)),
        "FIRSTNAME" => (Expression<Func<User, bool>>)((user) => user.FirstName.Contains(filterStr)),
        "LASTNAME" => (Expression<Func<User, bool>>)((user) => user.LastName.Contains(filterStr)),
        "GENDER" => (Expression<Func<User, bool>>)((user) => user.Gender.Contains(filterStr)),
        _ => (Expression<Func<User, bool>>)((user) => true)
      };

      var sorter = sortBy switch
      {
        "FIRSTNAME" => (Expression<Func<User, object>>)((user) => user.FirstName),
        "LASTNAME" => (Expression<Func<User, object>>)((user) => user.LastName),
        "GENDER" => (Expression<Func<User, object>>)((user) => user.Gender),
        _ => (Expression<Func<User, object>>)((user) => user.Email)
      };

      return await Users.AsQueryable().GetPage(pageNumber, pageSize, sortOrder, sorter, filter)
        .AsNoTracking().ToListAsync();
    }

    public async Task<bool> IsAdministrator(string email)
    {
      var user = await FindByEmailAsync(email);

      return await IsInRoleAsync(user, "Administrator");
    }
  }
}
