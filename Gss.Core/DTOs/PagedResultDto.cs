namespace Gss.Core.DTOs;

public sealed class PagedResultDto<T>
{
  public required PagedInfoDto PagedInfo { get; set; }
  public required IEnumerable<T> Items { get; set; }
  public required int TotalItemsCount { get; set; }

  public PagedResultDto<TDestination> Convert<TDestination>(Func<T, TDestination> mapper)
  {
    return new PagedResultDto<TDestination>
    {
      PagedInfo = PagedInfo,
      TotalItemsCount = TotalItemsCount,
      Items = Items.Select(mapper)
    };
  }
}