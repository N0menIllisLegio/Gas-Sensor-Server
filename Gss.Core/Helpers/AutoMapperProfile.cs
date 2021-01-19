using System;
using AutoMapper;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.DTOs.Role;
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

      CreateMap<IdentityRole<Guid>, RoleDto>();
      CreateMap<CreateRoleDto, IdentityRole<Guid>>();

      CreateMap<CreateUserDto, User>();

      CreateMap<Microcontroller, MicrocontrollerDto>();
      CreateMap<CreateMicrocontrollerDto, Microcontroller>();

      CreateMap<User, UserDto>();
      CreateMap<User, ExtendedUserDto>();
      CreateMap<CreateUserDto, User>();
      CreateMap<UpdateUserInfoDto, UpdateUserInfoModel>();
    }
  }
}
