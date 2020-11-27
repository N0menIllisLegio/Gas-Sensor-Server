using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gss.Core.Helpers
{
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
      var result = await base.CreateAsync(user);

      if (!result.Succeeded)
      {
        return result;
      }

      return await AddToRoleAsync(user, StandardRoleName);
    }
  }
}
