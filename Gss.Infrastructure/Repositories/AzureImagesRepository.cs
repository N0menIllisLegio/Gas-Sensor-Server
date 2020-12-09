using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;

namespace Gss.Infrastructure.Repositories
{
  public class AzureImagesRepository : IAzureImagesRepository
  {
    private readonly string _imageContainerPath;
    private readonly StorageSharedKeyCredential _storageCredentials;

    public AzureImagesRepository()
    {
      _imageContainerPath = "https://"
        + Settings.AzureImages.AccountName
        + ".blob.core.windows.net/"
        + Settings.AzureImages.ImagesContainer
        + "/";

      _storageCredentials =
        new StorageSharedKeyCredential(Settings.AzureImages.AccountName, Settings.AzureImages.AccountKey);
    }

    public async Task<Uri> UploadImage(Stream imageStream, string userID = null)
    {
      var blobUri = new Uri(_imageContainerPath + userID ?? Guid.NewGuid().ToString());
      var blobClient = new BlobClient(blobUri, _storageCredentials);

      await blobClient.DeleteIfExistsAsync();

      // TODO check if needed - imageStream.Position = 0;

      await blobClient.UploadAsync(imageStream);

      return blobUri;
    }

    public async Task DeleteImage(string userID)
    {
      if (String.IsNullOrEmpty(userID))
      {
        throw new ArgumentException();
      }

      var blobUri = new Uri(_imageContainerPath + userID);

      var blobClient = new BlobClient(blobUri, _storageCredentials);

      await blobClient.DeleteIfExistsAsync();
    }
  }
}
