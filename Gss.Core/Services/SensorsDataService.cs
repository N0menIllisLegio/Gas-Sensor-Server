using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gss.Core.DTOs.SensorData;
using Gss.Core.Exceptions;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Resources;

namespace Gss.Core.Services
{
  public class SensorsDataService: ISensorsDataService
  {
    readonly IUnitOfWork _unitOfWork;
    readonly UserManager _userManager;

    public SensorsDataService(IUnitOfWork unitOfWork, UserManager userManager)
    {
      _unitOfWork = unitOfWork;
      _userManager = userManager;
    }

    public async Task<List<SensorDataDto>> GetSensorData(string requestedByEmail, RequestSensorDataDto requestSensorDataDto)
    {
      if (requestSensorDataDto is null
        || requestSensorDataDto.MicrocontrollerID == Guid.Empty
        || requestSensorDataDto.SensorID == Guid.Empty
        || requestSensorDataDto.WatchingDates is null
        || requestSensorDataDto.WatchingDates.Count == 0
        || requestSensorDataDto.WatchingDates.Count > 5)
      {
        throw new AppException(String.Format(Messages.BadRequestErrorString), HttpStatusCode.BadRequest);
      }

      bool administratorClaim = await _userManager.IsAdministrator(requestedByEmail);

      var microcontroller = await _unitOfWork.Microcontrollers.GetFirstWhereAsync(microcontroller =>
        microcontroller.Id == requestSensorDataDto.MicrocontrollerID
        && (microcontroller.Public || administratorClaim || microcontroller.Owner.Email == requestedByEmail))
        ?? throw new AppException(String.Format(Messages.NotFoundErrorString, "Microcontroller"), HttpStatusCode.NotFound);

      var result = new List<SensorDataDto>();

      foreach (var watchingDate in requestSensorDataDto.WatchingDates)
      {
        var sensorData = await _unitOfWork.SensorsData.GetSensorDataByPeriod(requestSensorDataDto.MicrocontrollerID,
          requestSensorDataDto.SensorID, watchingDate, requestSensorDataDto.Period);

        result.AddRange(sensorData.Select(data => new SensorDataDto
        {
          WatchingDate = watchingDate,
          ValueReadTime = data.ValueReadTime,
          AverageSensorValue = data.AverageSensorValue
        }));
      }

      return result.OrderBy(sensorData => sensorData.ValueReadTime).ToList();
    }
  }
}
