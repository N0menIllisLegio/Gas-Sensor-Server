using Gss.Core.Enums;

namespace Gss.Core.Interfaces
{
  public interface IFilterCriterion
  {
    string PropertyName { get; set; }

    string Value { get; set; }

    FilterOperatorType OperatorType { get; set; }
  }
}
