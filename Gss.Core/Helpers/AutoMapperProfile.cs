using AutoMapper;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;
using Gss.Core.Models;

namespace Gss.Core.Helpers;

public class AutoMapperProfile : Profile
{
  // TODO: get rid of
  public AutoMapperProfile()
  {
      CreateMap<Sensor, SensorDto>();
      // TODO:.ForMember(dest => dest.SensorType, opt => opt.MapFrom(src => src.Type));
      CreateMap<CreateSensorDto, Sensor>();
      CreateMap<UpdateSensorDto, Sensor>();

      CreateMap<MicrocontrollerSensors, MicrocontrollerSensorDto>()
        .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Sensor.Id))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Sensor.Name))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Sensor.Description));
      // TODO:.ForMember(dest => dest.SensorType, opt => opt.MapFrom(src => src.Sensor.Type));

      CreateMap<Microcontroller, MicrocontrollerDto>()
        .ForMember(dest => dest.Sensors, opt => opt.MapFrom(src => src.MicrocontrollerSensors));
        // .ForMember(dest => dest.UserInfo, opt => opt.MapFrom(src => src.Owner));
      CreateMap<CreateMicrocontrollerDto, Microcontroller>()
        .ForMember(dest => dest.Key, opt => opt.MapFrom(src => CryptoHelper.GetHashString(src.Password)));
      CreateMap<UpdateMicrocontrollerDto, Microcontroller>()
        .ForMember(dest => dest.Key, opt => opt.MapFrom(src => CryptoHelper.GetHashString(src.Password)));

      CreateMap<MapMicrocontrollerModel, MapMicrocontrollerDto>();
    }
}