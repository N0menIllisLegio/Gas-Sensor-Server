using System.Threading.Tasks;
using Gss.Core.DTOs.File;

namespace Gss.Core.Interfaces
{
  public interface IFilesService
  {
    Task<FileDto> UploadImages(UploadFileDto uploadImagesDto);
    Task DeleteImage(DeleteFileDto deleteImageDto);
  }
}
