using System;
using System.Linq;
using AutoMapper;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.DTOs.Role;
using Gss.Core.DTOs.Sensor;
using Gss.Core.DTOs.SensorType;
using Gss.Core.DTOs.User;
using Gss.Core.Entities;
using Gss.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Gss.Core.Helpers
{
  public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
      CreateMap<SensorType, SensorTypeDto>();
      CreateMap<CreateSensorTypeDto, SensorType>();
      CreateMap<UpdateSensorTypeDto, SensorType>();

      CreateMap<Sensor, SensorDto>()
        .ForMember(dest => dest.SensorType, opt => opt.MapFrom(src => src.Type));
      CreateMap<CreateSensorDto, Sensor>();
      CreateMap<UpdateSensorDto, Sensor>();

      CreateMap<IdentityRole<Guid>, RoleDto>();
      CreateMap<CreateRoleDto, IdentityRole<Guid>>();
      CreateMap<UpdateRoleDto, IdentityRole<Guid>>();

      CreateMap<User, UserDto>();
      CreateMap<User, ExtendedUserDto>();
      CreateMap<CreateUserDto, User>();
      CreateMap<UpdateUserInfoDto, UpdateUserInfoModel>();
      CreateMap<UpdateUserInfoDto, User>();
      CreateMap<UpdateUserDto, User>();
      CreateMap<UpdateUserInfoModel, User>();

      CreateMap<Microcontroller, MicrocontrollerDto>()
        .ForMember(dest => dest.Sensors, opt => opt.MapFrom(src => src.MicrocontrollerSensors.Select(s => s.Sensor).ToList()))
        .ForMember(dest => dest.UserInfo, opt => opt.MapFrom(src => src.Owner));
      CreateMap<CreateMicrocontrollerDto, Microcontroller>();
      CreateMap<UpdateMicrocontrollerDto, Microcontroller>();
    }
  }
}
