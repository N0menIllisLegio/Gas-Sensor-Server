using AutoMapper;
using Gss.Core.DTOs.SensorType;
using Gss.Core.DTOs.User;
using Gss.Core.Entities;

namespace Gss.Core.Helpers
{
  public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
      CreateMap<User, ExtendedUserInfoDto>()
        .ForMember(destination => destination.FirstName, options => options.MapFrom(source => "KEKER"));

      CreateMap<SensorType, SensorTypeDto>();
      CreateMap<CreateSensorTypeDto, SensorType>();
    }
  }
}
