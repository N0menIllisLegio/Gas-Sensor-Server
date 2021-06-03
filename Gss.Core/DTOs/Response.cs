using System;
using System.Collections.Generic;
using System.Linq;

namespace Gss.Core.DTOs
{
  public class Response<T>
  {
    public Response()
    {
      Succeeded = true;
      Errors = new List<string>();
      Data = default;
    }

    public Response(T data)
    {
      Succeeded = true;
      Errors = new List<string>();
      Data = data;
    }

    public T Data { get; set; }
    public bool Succeeded { get; set; }
    public List<string> Errors { get; set; }

    public Response<T> AddError(string error, params string[] errorParams)
    {
      Succeeded = false;
      string fromattedError = String.Format(error, errorParams);

      Errors.Add(fromattedError);

      return this;
    }

    public Response<T> AddErrors(params string[] errors)
    {
      Succeeded = false;
      Errors.AddRange(errors);

      return this;
    }

    public Response<T> AddErrors(IEnumerable<string> errors)
    {
      return AddErrors(errors.ToArray());
    }
  }
}
