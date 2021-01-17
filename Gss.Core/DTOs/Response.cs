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
      Data = new List<T>();
      Errors = new List<string>();
      NewData = default;
    }

    public Response(IEnumerable<T> data)
    {
      Succeeded = true;
      Data = new List<T>(data);
      Errors = new List<string>();
    }

    public Response(T data)
    {
      Succeeded = true;
      Data = new List<T> { data };
      Errors = new List<string>();
      NewData = data;
    }

    public Response(ServiceResultDto<T> resultDto)
    {
      Succeeded = resultDto.Succeeded;
      Data = resultDto.Data;
      Errors = resultDto.Errors;
    }

    public T NewData { get; protected set; }
    public IEnumerable<T> Data { get; protected set; }
    public bool Succeeded { get; protected set; }
    public List<string> Errors { get; protected set; }

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
