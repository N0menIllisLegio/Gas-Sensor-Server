using System.Collections.Generic;

namespace Gss.Core.DTOs
{
  public class PagedResponse<T> : Response<T>
  {
    public PagedResponse()
    { }

    public PagedResponse(T data, int pageNumber, int pageSize)
      : base(data)
    {
      PageNumber = pageNumber;
      PageSize = pageSize;
    }

    public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize)
      : base(data)
    {
      PageNumber = pageNumber;
      PageSize = pageSize;
    }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public bool OrderedByAscendind { get; set; }
    public string OrderedBy { get; set; }
    public string Filter { get; set; }
    public string FilteredBy { get; set; }
  }
}
