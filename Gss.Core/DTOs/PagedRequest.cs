using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gss.Core.Enums;
using Gss.Core.Helpers;

namespace Gss.Core.DTOs
{
  public class PagedRequest: IValidatableObject
  {
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public string SortBy { get; set; }
    public SortOrder SortOrder { get; set; }

    public string FilterBy { get; set; }
    public string Filter { get; set; }

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

      if (Filter is null)
      {
        Filter = String.Empty;
      }

      SortBy = SortBy?.ToUpper();
      FilterBy = FilterBy?.ToUpper();

      return new List<ValidationResult>();
    }
  }
}
