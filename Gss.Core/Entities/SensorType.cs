using Gss.Core.Interfaces;

namespace Gss.Core.Entities;

public sealed class SensorType : IEntity
{
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public string? Icon { get; set; }
  public string? Units { get; set; } // like C, F,Hz, Db ?
}