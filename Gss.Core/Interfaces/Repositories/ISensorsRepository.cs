using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Repositories;

public interface ISensorsRepository : IRepositoryBase<Sensor>
{
    Task<Sensor?> FindSensorAsync(Guid sensorId);
    Task<int> UpdateSensorAsync(Guid sensorId, UpdateSensorDto updateSensorDto);
}