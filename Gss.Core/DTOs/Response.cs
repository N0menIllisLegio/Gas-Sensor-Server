using System.Collections.Generic;
using System.Linq;

namespace Gss.Core.DTOs
{
  public class Response<T>
  {
    public Response()
    { }

    public Response(IEnumerable<T> data)
    {
      Succeeded = true;
      Data = data;
    }

    public Response(T data)
    {
      Succeeded = true;
      Data = new List<T>
      {
        data
      };
    }

    public IEnumerable<T> Data { get; set; }
    public bool Succeeded { get; set; }
    public List<string> Errors { get; set; }

    public Response<T> AddErrors(params string[] errors)
    {
      Succeeded = false;
      Errors = new List<string>(errors);

      return this;
    }

    public Response<T> AddErrors(IEnumerable<string> errors)
    {
      return AddErrors(errors.ToArray());
    }
  }
}
