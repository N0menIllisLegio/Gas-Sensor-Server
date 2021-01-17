using Gss.Core.Enums;
using Gss.Core.Interfaces;

namespace Gss.Core.Models
{
  public class SortOption : ISortOption
  {
    public SortOrder Order { get; set; }
    public string PropertyName { get; set; }
  }
}
