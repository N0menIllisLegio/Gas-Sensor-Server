using Gss.Core.Entities;

namespace Gss.Core.Interfaces;

public enum SignInResult
{
    Failure,
    NotAllowed,
    LockedOut,
    Success
}

public interface IAuthenticationManager
{
    Task<SignInResult> CheckPasswordSignInAsync(User user, string password);
}