using System;
using System.IO;
using System.Threading.Tasks;

namespace Gss.Core.Interfaces
{
  public interface IAzureImagesRepository
  {
    Task<Uri> UploadImage(Stream imageStream, string userID = null);
    Task DeleteImage(string userID);
  }
}
