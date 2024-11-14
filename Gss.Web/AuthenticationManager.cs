using Gss.Core.Entities;
using Gss.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using SignInResult = Gss.Core.Interfaces.SignInResult;

namespace Gss.Web;

internal sealed class AuthenticationManager: IAuthenticationManager
{
    private readonly SignInManager<User> _signInManager;

    public AuthenticationManager(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<SignInResult> CheckPasswordSignInAsync(User user, string password)
    {
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        return signInResult.Succeeded
            ? SignInResult.Success
            : signInResult.IsLockedOut
                ? SignInResult.LockedOut
                : signInResult.IsNotAllowed
                    ? SignInResult.NotAllowed
                    : SignInResult.Failure;
    }
}
