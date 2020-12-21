using System;
using System.Collections.Generic;
using System.Linq;

namespace Gss.Core.DTOs
{
  public class ServiceResultDto<T>
  {
    public ServiceResultDto()
    {
      Data = new List<T>();
      Errors = new List<string>();
    }

    public ServiceResultDto(IEnumerable<T> data)
    {
      Data = new List<T>(data);
      Errors = new List<string>();
    }

    public ServiceResultDto(T data)
    {
      Data = new List<T> { data };
      Errors = new List<string>();
    }

    public List<T> Data { get; private set; }
    public bool Succeeded => Errors.Count == 0;
    public List<string> Errors { get; private set; }

    public ServiceResultDto<T> AddError(string error, params string[] errorParams)
    {
      string fromattedError = String.Format(error, errorParams);

      Errors.Add(fromattedError);

      return this;
    }

    public ServiceResultDto<T> AddErrors(params string[] errors)
    {
      Errors.AddRange(errors);

      return this;
    }

    public ServiceResultDto<T> AddErrors(IEnumerable<string> errors)
    {
      return AddErrors(errors.ToArray());
    }
  }
}
