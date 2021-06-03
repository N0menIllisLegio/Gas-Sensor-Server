using System;
using System.Net;

namespace Gss.Core.Exceptions
{
  public class AppException : Exception
  {
    public AppException(string message, HttpStatusCode errorCode)
      : base(message)
    {
      ErrorCode = errorCode;
    }

    public HttpStatusCode ErrorCode { get; set; }
  }
}
