using System.Threading.Tasks;
using Gss.Core.DTOs.File;

namespace Gss.Core.Interfaces.Services
{
  public interface IFilesService
  {
    Task<FileDto> UploadImageAsync(UploadFileDto uploadImagesDto);
    Task DeleteImageAsync(DeleteFileDto deleteImageDto);
  }
}
