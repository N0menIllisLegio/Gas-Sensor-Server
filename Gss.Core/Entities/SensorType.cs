using Gss.Core.Interfaces;
using Gss.Core.Utils;

namespace Gss.Core.Entities;

public sealed class SensorType : IEntity
{
    [AllowOrdering] public required string Name { get; set; }

    public string? Icon { get; set; }

    [AllowOrdering] public string? Units { get; set; }

    [AllowOrdering] public Guid Id { get; set; }
}