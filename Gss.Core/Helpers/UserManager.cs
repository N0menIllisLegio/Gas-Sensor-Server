using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.Entities;
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

    public async Task<IEnumerable<User>> GetPage(int pageSize, int pageNumber,
      bool orderAsc, string orderBy, string filterBy = null, string filterStr = null)
    {
      var filter = filterBy switch
      {
        "Email" => (Expression<Func<User, bool>>)((user) => user.Email.Contains(filterStr)),
        "FirstName" => (Expression<Func<User, bool>>)((user) => user.FirstName.Contains(filterStr)),
        "LastName" => (Expression<Func<User, bool>>)((user) => user.LastName.Contains(filterStr)),
        "Gender" => (Expression<Func<User, bool>>)((user) => user.Gender.Contains(filterStr)),
        _ => (Expression<Func<User, bool>>)((user) => true)
      };

      return await GetPage(pageSize, pageNumber, orderAsc, orderBy, Users.Where(filter));
    }

    public async Task<IEnumerable<User>> GetPage(int pageSize, int pageNumber,
      bool orderAsc, string orderBy, IQueryable<User> users = null)
    {
      if (users is null)
      {
        users = Users;
      }

      var order = orderBy switch
      {
        "FirstName" => (Expression<Func<User, string>>)((user) => user.FirstName),
        "LastName" => (Expression<Func<User, string>>)((user) => user.LastName),
        "Gender" => (Expression<Func<User, string>>)((user) => user.Gender),
        _ => (Expression<Func<User, string>>)((user) => user.Email)
      };

      users = orderAsc
        ? users.OrderBy(order)
        : users.OrderByDescending(order);

      return await users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }
  }
}
