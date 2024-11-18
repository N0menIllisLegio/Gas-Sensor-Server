using Gss.Core.Helpers;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities;

public sealed class SensorType : IEntity
{
  [AllowOrdering]
  public Guid Id { get; set; }

  [AllowOrdering]
  public required string Name { get; set; }
  public string? Icon { get; set; }

  [AllowOrdering]
  public string? Units { get; set; }
}