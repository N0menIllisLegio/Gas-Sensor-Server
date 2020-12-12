using System;
using System.IO;
using System.Threading.Tasks;
using Gss.Core.DTOs;

namespace Gss.Core.Interfaces
{
  public interface IAzureImagesRepository
  {
    Task<Response<Uri>> UploadImage(Stream imageStream, string imageExtension, string userID = null);
    Task DeleteImage(string userID);
  }
}
