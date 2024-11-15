using Gss.Core.Entities;
using Gss.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Gss.Web;

internal sealed class AuthenticationManager: IAuthenticationManager
{
    private readonly SignInManager<User> _signInManager;

    public AuthenticationManager(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<UserSignInResult> CheckPasswordSignInAsync(User user, string password)
    {
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        return signInResult.Succeeded
            ? UserSignInResult.Success
            : signInResult.IsLockedOut
                ? UserSignInResult.LockedOut
                : signInResult.IsNotAllowed
                    ? UserSignInResult.NotAllowed
                    : UserSignInResult.Failure;
    }
}
