using Gss.Core.DTOs.SensorData;
using Gss.Core.Exceptions;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Resources;

namespace Gss.Core.Services;

public class SensorsDataService : ISensorsDataService
{
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public SensorsDataService(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Dictionary<DateOnly, List<SensorDataDto>>> GetSensorDataAsync(
        RequestSensorDataDto requestSensorDataDto, CancellationToken cancellationToken = default)
    {
        var microcontrollers = await _unitOfWork.Microcontrollers.CountAsync(
            mc => mc.MicrocontrollerSensors.Any(x => x.Id == requestSensorDataDto.MicrocontrollerSensorId) &&
                  (_currentUser.IsAdministrator || mc.Public || mc.OwnerId == _currentUser.Id),
            cancellationToken);

        if (microcontrollers != 1)
            throw new NotFoundException(string.Format(Messages.NotFoundErrorString, "Microcontroller"));

        var result = new Dictionary<DateOnly, List<SensorDataDto>>();

        foreach (var watchingDate in requestSensorDataDto.WatchingDates)
        {
            var sensorData = await _unitOfWork.SensorsData.GetSensorDataByPeriodAsync(
                requestSensorDataDto.MicrocontrollerSensorId, watchingDate, requestSensorDataDto.Period,
                cancellationToken);

            result[watchingDate] = sensorData
                .Select(x => new SensorDataDto
                {
                    ReadTime = DateTime.SpecifyKind(x.ReadTime, DateTimeKind.Utc),
                    AverageValue = Math.Floor(x.AverageValue),
                    WatchingDate = x.WatchingDate
                })
                .OrderBy(x => x.ReadTime)
                .ToList();
        }

        return result;
    }
}