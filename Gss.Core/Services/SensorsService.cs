using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Mappers;
using Gss.Core.Resources;

namespace Gss.Core.Services;

public class SensorsService : ISensorsService
{
    private const string Sensor = "Sensor";

    private readonly IUnitOfWork _unitOfWork;

    public SensorsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultDto<SensorDto>> GetAllSensorsAsync(PagedInfoDto pagedInfoDto,
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await _unitOfWork.Sensors.GetPagedResultAsync(pagedInfoDto, cancellationToken);

        return pagedResult.Convert(x => x.MapToDto());
    }

    public async Task<SensorDto> GetSensorAsync(Guid sensorId, CancellationToken cancellationToken = default)
    {
        var sensor = await _unitOfWork.Sensors.FindSensorAsync(sensorId, cancellationToken);

        if (sensor is null)
            throw new NotFoundException(string.Format(Messages.NotFoundErrorString, Sensor));

        return sensor.MapToDto();
    }

    public async Task<SensorDto> CreateSensorAsync(CreateSensorDto createSensorDto,
        CancellationToken cancellationToken = default)
    {
        var sensorId = _unitOfWork.Sensors.Add(new Sensor
        {
            Name = createSensorDto.Name,
            Description = createSensorDto.Description,
            TypeId = createSensorDto.TypeId
        });

        var success = await _unitOfWork.SaveAsync(cancellationToken);

        if (!success)
            throw new AppException(string.Format(Messages.CreationFailedErrorString, Sensor));

        return await GetSensorAsync(sensorId, cancellationToken);
    }

    public async Task UpdateSensorAsync(Guid sensorId, UpdateSensorDto updateSensorDto,
        CancellationToken cancellationToken = default)
    {
        var updatedCount = await _unitOfWork.Sensors.UpdateSensorAsync(sensorId, updateSensorDto, cancellationToken);

        if (updatedCount == 0)
            throw new NotFoundException(string.Format(Messages.NotFoundErrorString, Sensor));
    }

    public async Task DeleteSensorAsync(Guid sensorId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Sensors.RemoveAsync(sensorId, cancellationToken);
    }
}