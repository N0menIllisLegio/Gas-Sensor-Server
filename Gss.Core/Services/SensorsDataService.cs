using Gss.Core.DTOs.SensorData;
using Gss.Core.Exceptions;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Resources;

namespace Gss.Core.Services;

public class SensorsDataService: ISensorsDataService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUser _currentUser;

  public SensorsDataService(IUnitOfWork unitOfWork, ICurrentUser currentUser)
  {
    _unitOfWork = unitOfWork;
    _currentUser = currentUser;
  }

  public async Task<List<SensorDataDto>> GetSensorDataAsync(RequestSensorDataDto requestSensorDataDto)
  {
    _ = await _unitOfWork.Microcontrollers.FirstOrDefaultAsync(
        mc => mc.MicrocontrollerSensors.Any(x => x.Id == requestSensorDataDto.MicrocontrollerSensorId)
          && (_currentUser.IsAdministrator || mc.Public || mc.OwnerId == _currentUser.Id))
      ?? throw new NotFoundException(string.Format(Messages.NotFoundErrorString, "Microcontroller"));

    var result = new List<SensorDataDto>();

    foreach (var watchingDate in requestSensorDataDto.WatchingDates)
    {
      var sensorData = await _unitOfWork.SensorsData.GetSensorDataByPeriodAsync(
        requestSensorDataDto.MicrocontrollerSensorId, watchingDate, requestSensorDataDto.Period);

      result.AddRange(sensorData);
    }

    result.ForEach(x =>
    {
      x.ReadTime = DateTime.SpecifyKind(x.ReadTime, DateTimeKind.Utc);
      x.AverageValue = Math.Floor(x.AverageValue);
    });

    return result.OrderBy(sensorData => sensorData.ReadTime).ToList();
  }
}