using Gss.Core.Enums;
using Gss.Core.Interfaces;

namespace Gss.Core.Models
{
  public class FilterCriterion : IFilterCriterion
  {
    public string PropertyName { get; set; }
    public string Value { get; set; }
    public FilterOperatorType OperatorType { get; set; }
  }
}
