using System.ComponentModel;

namespace Gss.Core.DTOs;

public sealed class SortOption
{
  public bool IsAscending { get; set; }
  public required string PropertyName { get; set; }
}

public sealed class PagedInfoDto
{
  [DefaultValue(1)]
  public int PageNumber { get; set; }

  [DefaultValue(20)]
  public int PageSize { get; set; }

  [DefaultValue("")]
  public required string SearchString { get; set; }

  [DefaultValue(null)]
  public List<SortOption>? SortOptions { get; set; }
}