using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces
{
  public interface IMicrocontrollerService
  {
    Task<Response<Microcontroller>> AddMicrocontroller(CreateMicrocontrollerDto dto, User user);

    Task<Response<Microcontroller>> UpdateMicrocontroller(UpdateMicrocontrollerDto dto, User user);

    Task<Response<Microcontroller>> UpdateMicrocontroller(UpdateMicrocontrollerDto dto);

    Task<Response<Microcontroller>> DeleteMicrocontroller(string microcontrollerID, User user);

    Task<Response<Microcontroller>> DeleteMicrocontroller(string microcontrollerID);
  }
}
