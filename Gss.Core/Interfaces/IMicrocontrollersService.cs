using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Enums;

namespace Gss.Core.Interfaces
{
  public interface IMicrocontrollersService
  {
    Task<(ServiceResultDto<Microcontroller> result, int microcontrollersCount, bool displaySensitiveInfo)> GetUserMicrocontrollers(string userID, string requestedByEmail,
      int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string sortBy = "",
      string filterBy = null, string filterStr = "");

    Task<(ServiceResultDto<Microcontroller> result, int totalQueriedMicrocontrollersCount)> GetPublicMicrocontrollers(
      int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string sortBy = "",
      string filterBy = null, string filterStr = "");

    Task<(ServiceResultDto<Microcontroller> result, int totalQueriedMicrocontrollersCount)> GetAllMicrocontrollers(
      int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string sortBy = "",
      string filterBy = null, string filterStr = "");

    Task<(ServiceResultDto<Microcontroller> result, bool displaySensitiveInfo)> GetMicrocontroller(Guid microcontrollerID, string requestedByEmail);

    Task<ServiceResultDto<Microcontroller>> AddMicrocontroller(CreateMicrocontrollerDto dto, string ownerEmail);

    Task<ServiceResultDto<Microcontroller>> UpdateMicrocontroller(UpdateMicrocontrollerDto dto, string ownerEmail);

    Task<ServiceResultDto<Microcontroller>> ChangeMicrocontrollerOwner(Guid microcontrollerID, string newOwnerID);

    Task<ServiceResultDto<Microcontroller>> DeleteMicrocontroller(Guid microcontrollerID, string ownerEmail);
  }
}
