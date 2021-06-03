using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Gss.Core.DTOs.File;
using Gss.Core.Exceptions;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Resources;

namespace Gss.Core.Services
{
  public class FilesService : IFilesService
  {
    private readonly IUnitOfWork _unitOfWork;

    public FilesService(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    public async Task DeleteImageAsync(DeleteFileDto deleteImageDto)
    {
      var imageUri = deleteImageDto.FileUrl;

      if (!_unitOfWork.AzureFiles.ValidateUri(imageUri))
      {
        throw new AppException(Messages.InvalidUriErrorString, HttpStatusCode.BadRequest);
      }

      await _unitOfWork.AzureFiles.DeleteImageAsync(imageUri);
    }

    public async Task<FileDto> UploadImageAsync(UploadFileDto uploadImagesDto)
    {
      string imageExtension = Path.GetExtension(uploadImagesDto.FileForm.FileName);

      if (!Settings.AzureImages.SupportedExtensions.Contains(imageExtension))
      {
        string error = String.Format(Messages.UnsupportedImageExtensionErrorString,
          String.Join(", ", Settings.AzureImages.SupportedExtensions));

        throw new AppException(error, HttpStatusCode.BadRequest);
      }

      var uri = await _unitOfWork.AzureFiles
        .AddImageAsync(uploadImagesDto.FileForm.OpenReadStream(), imageExtension);

      if (uri is null)
      {
        throw new AppException(Messages.AzureUploadFailedErrorString, HttpStatusCode.ServiceUnavailable);
      }

      return new FileDto { FileUrl = uri };
    }
  }
}
