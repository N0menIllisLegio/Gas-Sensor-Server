using System;
using System.IO;
using System.Threading.Tasks;

namespace Gss.Core.Interfaces
{
  public interface IAzureFilesRepository
  {
    Task<Uri> AddImageAsync(Stream imageStream, string imageExtension);
    Task DeleteImageAsync(Uri imageUri);

    bool ValidateUri(Uri validatingUri);
  }
}
