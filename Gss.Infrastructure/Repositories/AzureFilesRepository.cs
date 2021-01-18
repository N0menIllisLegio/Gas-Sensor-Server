using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;

namespace Gss.Infrastructure.Repositories
{
  public class AzureFilesRepository : IAzureFilesRepository
  {
    private readonly string _imageContainerPath;
    private readonly StorageSharedKeyCredential _storageCredentials;

    public AzureFilesRepository()
    {
      _imageContainerPath = "https://"
        + Settings.AzureImages.AccountName
        + ".blob.core.windows.net/"
        + Settings.AzureImages.ImagesContainer
        + "/";

      _storageCredentials =
        new StorageSharedKeyCredential(Settings.AzureImages.AccountName, Settings.AzureImages.AccountKey);
    }

    public async Task<Uri> AddImageAsync(Stream imageStream, string imageExtension)
    {
      var imageUri = new Uri($"{_imageContainerPath}{Guid.NewGuid()}.{imageExtension}");
      var blobClient = new BlobClient(imageUri, _storageCredentials);

      try
      {
        await blobClient.DeleteIfExistsAsync();
        await blobClient.UploadAsync(imageStream);
      }
      catch
      {
        return null;
      }

      return imageUri;
    }

    public async Task DeleteImageAsync(Uri imageUri)
    {
      var blobClient = new BlobClient(imageUri, _storageCredentials);

      await blobClient.DeleteIfExistsAsync();
    }

    public bool ValidateUri(Uri validatingUri)
    {
      return validatingUri.ToString().Contains(_imageContainerPath);
    }
  }
}
