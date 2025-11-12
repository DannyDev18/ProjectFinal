using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Project.Application.Dtos;
using Project.Application.Services;
using Project.Application.Settings;

namespace Project.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<ImageService> _logger;
        private readonly CloudinarySettings _settings;
        private readonly Cloudinary _directCloudinary; // Nueva instancia con credenciales correctas

        public ImageService(Cloudinary cloudinary, ILogger<ImageService> logger, IOptions<CloudinarySettings> settings)
        {
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));

            // CREAR INSTANCIA DIRECTA CON CREDENCIALES CORRECTAS
            var directAccount = new Account(
               "dvdzabq8x",     // CloudName
                 "411446253291967",         // ApiKey  
              "EftulyDBMhZG7IfkLf2Pt_rMN1E"         // ApiSecret CORREGIDO
            );
           _directCloudinary = new Cloudinary(directAccount);

            _logger.LogInformation("ImageService initialized with CloudName: {CloudName}", _settings.CloudName);
        _logger.LogInformation("Direct Cloudinary instance created with correct credentials");
        }

    public async Task<ImageUploadResultDto> UploadImageAsync(IFormFile file, string folder = "products")
        {
            try
      {
         if (file == null || file.Length == 0)
   {
           _logger.LogWarning("Upload attempt with null or empty file");
   return new ImageUploadResultDto
          {
             Success = false,
   ErrorMessage = "No file provided or file is empty"
          };
   }

           _logger.LogInformation("=== STARTING IMAGE UPLOAD ===");
  _logger.LogInformation("File: {FileName}, Size: {Size} bytes, ContentType: {ContentType}", 
               file.FileName, file.Length, file.ContentType);

          // Validación de tipo de archivo
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
    if (!allowedTypes.Contains(file.ContentType.ToLower()))
     {
  _logger.LogError("Invalid file type: {ContentType}. Allowed: {AllowedTypes}", 
             file.ContentType, string.Join(", ", allowedTypes));
         return new ImageUploadResultDto
          {
            Success = false,
         ErrorMessage = $"Invalid file type: {file.ContentType}. Only JPEG, PNG, GIF, and WebP are allowed."
     };
      }

          // Validación de tamaño (máximo 10MB)
      const int maxSizeInBytes = 10 * 1024 * 1024;
       if (file.Length > maxSizeInBytes)
  {
          _logger.LogError("File size {FileSize} exceeds limit {MaxSize}", file.Length, maxSizeInBytes);
  return new ImageUploadResultDto
        {
        Success = false,
         ErrorMessage = $"File size {file.Length / (1024 * 1024)} MB exceeds 10MB limit"
        };
      }

    using var stream = file.OpenReadStream();

                // USAR INSTANCIA DIRECTA CON CREDENCIALES CORRECTAS
                var uploadParams = new ImageUploadParams
         {
          File = new FileDescription(file.FileName, stream),
           Folder = folder, // Agregar folder para organización
         UseFilename = false,
       UniqueFilename = true,
Overwrite = false
  };

    _logger.LogInformation("Uploading to Cloudinary with direct credentials...");
        _logger.LogInformation("Upload params: Folder={Folder}, UseFilename={UseFilename}, UniqueFilename={UniqueFilename}", 
 uploadParams.Folder, uploadParams.UseFilename, uploadParams.UniqueFilename);

        var uploadResult = await _directCloudinary.UploadAsync(uploadParams);

    _logger.LogInformation("=== CLOUDINARY RESPONSE ===");
_logger.LogInformation("StatusCode: {StatusCode}", uploadResult.StatusCode);
                _logger.LogInformation("PublicId: {PublicId}", uploadResult.PublicId);
                _logger.LogInformation("SecureUrl: {SecureUrl}", uploadResult.SecureUrl);
    _logger.LogInformation("Error: {Error}", uploadResult.Error?.Message);

      if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
              {
           _logger.LogInformation("? Upload successful!");
     _logger.LogInformation("Image uploaded successfully - PublicId: {PublicId}, URL: {SecureUrl}", 
   uploadResult.PublicId, uploadResult.SecureUrl);

        return new ImageUploadResultDto
 {
          Success = true,
       PublicId = uploadResult.PublicId,
            SecureUrl = uploadResult.SecureUrl?.ToString() ?? string.Empty,
           Url = uploadResult.Url?.ToString() ?? string.Empty,
            Format = uploadResult.Format ?? string.Empty,
    ResourceType = uploadResult.ResourceType ?? string.Empty,
        Bytes = uploadResult.Bytes,
       Width = uploadResult.Width,
            Height = uploadResult.Height,
           CreatedAt = uploadResult.CreatedAt
        };
      }
           else
           {
       string errorMessage = uploadResult.Error?.Message ?? "Upload failed";
        _logger.LogError("? Upload failed!");
              _logger.LogError("StatusCode: {StatusCode}", uploadResult.StatusCode);
       _logger.LogError("Error Message: {ErrorMessage}", errorMessage);
      _logger.LogError("Error Details: {@ErrorDetails}", uploadResult.Error);

   return new ImageUploadResultDto
               {
    Success = false,
           ErrorMessage = $"Cloudinary upload failed: {errorMessage} (Status: {uploadResult.StatusCode})"
     };
       }
         }
  catch (Exception ex)
      {
          _logger.LogError(ex, "?? Exception during image upload");
                _logger.LogError("Exception Type: {ExceptionType}", ex.GetType().Name);
      _logger.LogError("Exception Message: {ExceptionMessage}", ex.Message);
      _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);

       return new ImageUploadResultDto
                {
      Success = false,
         ErrorMessage = $"Upload exception: {ex.Message}"
        };
  }
        }

        public async Task<ImageUploadResultDto> UploadImageAsync(Stream imageStream, string fileName, string folder = "products")
        {
            try
         {
                if (imageStream == null || imageStream.Length == 0)
         {
         return new ImageUploadResultDto
    {
    Success = false,
     ErrorMessage = "No image stream provided or stream is empty"
     };
    }

         var uploadParams = new ImageUploadParams
            {
        File = new FileDescription(fileName, imageStream),
         Folder = folder,
        UseFilename = false,
     UniqueFilename = true,
 Overwrite = false
      };

    var uploadResult = await _directCloudinary.UploadAsync(uploadParams);

    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
       {
           return new ImageUploadResultDto
        {
    Success = true,
    PublicId = uploadResult.PublicId,
       SecureUrl = uploadResult.SecureUrl?.ToString() ?? string.Empty,
         Url = uploadResult.Url?.ToString() ?? string.Empty,
            Format = uploadResult.Format ?? string.Empty,
        ResourceType = uploadResult.ResourceType ?? string.Empty,
        Bytes = uploadResult.Bytes,
     Width = uploadResult.Width,
Height = uploadResult.Height,
          CreatedAt = uploadResult.CreatedAt
        };
       }
    else
       {
     return new ImageUploadResultDto
      {
      Success = false,
         ErrorMessage = uploadResult.Error?.Message ?? "Stream upload failed"
            };
            }
            }
            catch (Exception ex)
          {
   _logger.LogError(ex, "Exception during stream upload");
           return new ImageUploadResultDto
             {
          Success = false,
     ErrorMessage = $"Stream upload failed: {ex.Message}"
   };
     }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
  {
            try
  {
       if (string.IsNullOrWhiteSpace(publicId))
                {
      _logger.LogWarning("Attempted to delete image with empty public ID");
                    return false;
  }

                _logger.LogInformation("Deleting image with PublicId: {PublicId}", publicId);

                var deletionParams = new DeletionParams(publicId);
       var result = await _directCloudinary.DestroyAsync(deletionParams);

          _logger.LogInformation("Delete result: Status={Status}, Result={Result}", result.StatusCode, result.Result);

         if (result.StatusCode == System.Net.HttpStatusCode.OK && result.Result == "ok")
  {
             _logger.LogInformation("? Image deleted successfully: {PublicId}", publicId);
     return true;
  }
else
   {
   _logger.LogWarning("? Failed to delete image: {PublicId}, Result: {Result}", publicId, result.Result);
      return false;
          }
      }
catch (Exception ex)
      {
    _logger.LogError(ex, "Exception during image deletion: {PublicId}", publicId);
     return false;
   }
      }

     public async Task<bool> DeleteImageByUrlAsync(string imageUrl)
        {
  try
      {
     var publicId = ExtractPublicIdFromUrl(imageUrl);
                if (string.IsNullOrWhiteSpace(publicId))
  {
        _logger.LogWarning("Could not extract public ID from URL: {Url}", imageUrl);
      return false;
     }

            return await DeleteImageAsync(publicId);
       }
            catch (Exception ex)
      {
    _logger.LogError(ex, "Exception during delete by URL: {Url}", imageUrl);
   return false;
            }
    }

    public string ExtractPublicIdFromUrl(string imageUrl)
        {
          try
        {
                if (string.IsNullOrWhiteSpace(imageUrl))
     return string.Empty;

     _logger.LogInformation("Extracting public ID from URL: {Url}", imageUrl);

       var uri = new Uri(imageUrl);
     var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

    _logger.LogInformation("URL segments: {Segments}", string.Join(" | ", segments));

   if (segments.Length >= 4)
        {
            // Obtener el public_id (puede incluir carpetas)
     var publicIdWithExtension = string.Join("/", segments.Skip(3));
          var lastDotIndex = publicIdWithExtension.LastIndexOf('.');
   if (lastDotIndex > 0)
       {
          var extractedId = publicIdWithExtension.Substring(0, lastDotIndex);
          _logger.LogInformation("Extracted public ID: {PublicId}", extractedId);
        return extractedId;
     }
        
         _logger.LogInformation("Extracted public ID (no extension): {PublicId}", publicIdWithExtension);
     return publicIdWithExtension;
   }

        _logger.LogWarning("Could not extract public ID - insufficient segments");
        return string.Empty;
   }
        catch (Exception ex)
    {
        _logger.LogError(ex, "Exception extracting public ID from URL: {Url}", imageUrl);
        return string.Empty;
    }
}

        public async Task<IEnumerable<ImageUploadResultDto>> UploadMultipleImagesAsync(IEnumerable<IFormFile> files, string folder = "products")
    {
      var results = new List<ImageUploadResultDto>();

    foreach (var file in files)
  {
           var result = await UploadImageAsync(file, folder);
             results.Add(result);
        }

 return results;
        }
    }
}