using System.Net;

namespace Gss.Core.Exceptions;

public sealed class UserInputException : AppException
{
    public UserInputException(string message)
        : base(message, HttpStatusCode.BadRequest)
    {
    }
}