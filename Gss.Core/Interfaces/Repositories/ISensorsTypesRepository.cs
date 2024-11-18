using Gss.Core.DTOs.SensorType;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Repositories;

public interface ISensorsTypesRepository : IRepositoryBase<SensorType>
{
    Task<int> UpdateSensorTypeAsync(Guid sensorTypeId, UpdateSensorTypeDto updateSensorTypeDto);
}