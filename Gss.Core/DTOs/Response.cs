using System.Collections.Generic;

namespace Gss.Core.DTOs
{
  public class Response<T>
  {
    public Response()
    { }

    public Response(params string[] errors)
    {
      Succeeded = false;
      Errors = new List<string>(errors);
    }

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
  }
}
