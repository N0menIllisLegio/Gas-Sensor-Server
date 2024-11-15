using Gss.Core.Entities;

namespace Gss.Core.Interfaces;

public enum UserSignInResult
{
    Failure,
    NotAllowed,
    LockedOut,
    Success
}

public interface IAuthenticationManager
{
    Task<UserSignInResult> CheckPasswordSignInAsync(User user, string password);
}