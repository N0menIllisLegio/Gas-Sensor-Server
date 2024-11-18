using Gss.Core.Enums;
using Gss.Core.Interfaces;

namespace Gss.Core.Models;

public class SortOption : ISortOption
{
  // TODO: remove
  public SortOrder Order { get; set; }
  public bool IsAscending { get; set; }
  public required string PropertyName { get; set; }
}