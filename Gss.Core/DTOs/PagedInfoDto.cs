using System.ComponentModel.DataAnnotations;
using Gss.Core.Helpers;
using Gss.Core.Models;

namespace Gss.Core.DTOs;

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

      if (PageSize < 10)
      {
        PageSize = 10;
      }
      else if (PageSize > 50)
      {
        PageSize = 50;
      }

      return new List<ValidationResult>();
    }
}