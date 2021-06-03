using Gss.Core.Enums;

namespace Gss.Core.Interfaces
{
  public interface ISortOption
  {
    SortOrder Order { get; set; }
    string PropertyName { get; set; }
  }
}
