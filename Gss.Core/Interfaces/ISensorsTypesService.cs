using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Enums;

namespace Gss.Core.Interfaces
{
  public interface ISensorsTypesService
  {
    Task<(ServiceResultDto<SensorType> result, int totalQueriedSensorsTypesCount)> GetAllSensorsTypes(
      int pageNumber, int pageSize, SortOrder sortOrder = SortOrder.None, string filterStr = "");

    Task<ServiceResultDto<SensorType>> GetSensorType(string sensorTypeID);
  }
}
