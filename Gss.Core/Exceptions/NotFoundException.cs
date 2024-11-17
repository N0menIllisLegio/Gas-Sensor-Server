using System.Net;

namespace Gss.Core.Exceptions;

public sealed class NotFoundException: AppException
{
    public NotFoundException(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }
}
