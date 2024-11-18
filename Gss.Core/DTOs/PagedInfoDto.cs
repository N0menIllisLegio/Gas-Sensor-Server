using Gss.Core.Models;

namespace Gss.Core.DTOs;

public class PagedInfoDto
{
  public int PageNumber { get; set; }
  public int PageSize { get; set; }

  public string SearchString { get; set; }

  public List<SortOption> SortOptions { get; set; }

  public List<FilterCriterion> Filters { get; set; }
}