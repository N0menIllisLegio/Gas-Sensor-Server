namespace Gss.Core.DTOs;

public sealed class SortOption
{
  public bool IsAscending { get; set; }
  public required string PropertyName { get; set; }
}

public sealed class PagedInfoDto
{
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
  public required string SearchString { get; set; }
  public List<SortOption>? SortOptions { get; set; }
}