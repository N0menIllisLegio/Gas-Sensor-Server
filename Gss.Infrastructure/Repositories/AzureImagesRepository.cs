using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Gss.Core.DTOs;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Resources;

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

    public async Task<Response<Uri>> UploadImage(Stream imageStream, string imageExtension, string userID = null)
    {
      if (!Settings.AzureImages.SupportedExtensions.Contains(imageExtension))
      {
        string error = String.Format(Messages.UnsupportedImageExtensionErrorString,
          String.Join(", ", Settings.AzureImages.SupportedExtensions));

        return new Response<Uri>()
          .AddErrors(error);
      }

      var blobUri = new Uri($"{_imageContainerPath}{userID ?? Guid.NewGuid().ToString()}.{imageExtension}");
      var blobClient = new BlobClient(blobUri, _storageCredentials);

      try
      {
        await blobClient.DeleteIfExistsAsync();

        await blobClient.UploadAsync(imageStream);
      }
      catch
      {
        return new Response<Uri>()
          .AddErrors(Messages.AzureUploadFailedErrorString);
      }

      return new Response<Uri>(blobUri);
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
