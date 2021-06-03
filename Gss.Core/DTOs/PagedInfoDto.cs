using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gss.Core.Helpers;
using Gss.Core.Models;

namespace Gss.Core.DTOs
{
  public class PagedInfoDto: IValidatableObject
  {
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public string SearchString { get; set; }

    public List<SortOption> SortOptions { get; set; }

    public List<FilterCriterion> Filters { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (PageNumber <= 0)
      {
        PageNumber = 1;
      }

      if (PageSize < Settings.MinimumItemsPerPage)
      {
        PageSize = Settings.MinimumItemsPerPage;
      }
      else if (PageSize > Settings.MaximumItemsPerPage)
      {
        PageSize = Settings.MaximumItemsPerPage;
      }

      return new List<ValidationResult>();
    }
  }
}
