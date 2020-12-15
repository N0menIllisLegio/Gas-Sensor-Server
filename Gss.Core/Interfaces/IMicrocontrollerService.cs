using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Enums;

namespace Gss.Core.Interfaces
{
  public interface IMicrocontrollerService
  {
    Task<Response<Microcontroller>> AddMicrocontroller(CreateMicrocontrollerDto dto, string ownerEmail);

    Task<Response<Microcontroller>> UpdateMicrocontroller(UpdateMicrocontrollerDto dto, string ownerEmail);

    Task<Response<Microcontroller>> ChangeMicrocontrollerOwner(string microcontrollerID, string newOwnerID);

    Task<Response<Microcontroller>> DeleteMicrocontroller(string microcontrollerID, string ownerEmail);

    Task<Response<Microcontroller>> GetMicrocontroller(string microcontrollerID);

    Task<PagedResponse<Microcontroller>> GetUserMicrocontrollers(string userID,
      int pageNumber, int pageSize,
      SortOrder orderAsc = SortOrder.None, string sortBy = "",
      string filterBy = null, string filter = "");

    Task<PagedResponse<Microcontroller>> GetPublicMicrocontrollers(
      int pageNumber, int pageSize,
      SortOrder orderAsc = SortOrder.None, string sortBy = "",
      string filterBy = null, string filter = "");

    Task<PagedResponse<Microcontroller>> GetAllMicrocontrollers(
      int pageNumber, int pageSize,
      SortOrder orderAsc = SortOrder.None, string sortBy = "",
      string filterBy = null, string filter = "");
  }
}
