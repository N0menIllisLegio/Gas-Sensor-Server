using System;
using System.IO;
using System.Threading.Tasks;

namespace Gss.Core.Interfaces
{
  public interface IAzureFilesRepository
  {
    Task<Uri> AddImage(Stream imageStream, string imageExtension);
    Task DeleteImage(Uri imageUri);

    bool ValidateUri(Uri validatingUri);
  }
}
