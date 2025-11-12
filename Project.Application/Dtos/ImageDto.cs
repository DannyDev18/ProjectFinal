using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Application.Dtos
{
  public class ImageUploadResultDto
    {
        public string PublicId { get; set; } = string.Empty;
        public string SecureUrl { get; set; } = string.Empty;
 public string Url { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
   public string ResourceType { get; set; } = string.Empty;
        public long Bytes { get; set; }
   public int Width { get; set; }
        public int Height { get; set; }
    public DateTime CreatedAt { get; set; }
      public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ImageDeleteResultDto
    {
      public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
   public string PublicId { get; set; } = string.Empty;
    }

    public class ProductImageUploadDto
    {
      public int ProductId { get; set; }
        public IFormFile Image { get; set; } = null!;
    }

    public class ProductMultipleImagesUploadDto
{
        public int ProductId { get; set; }
        public IEnumerable<IFormFile> Images { get; set; } = new List<IFormFile>();
    }

    // DTOs para ImagesController
    public class DeleteImageByUrlRequestDto
    {
   [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
    public string ImageUrl { get; set; } = string.Empty;
    }

    public class ExtractPublicIdRequestDto
    {
     [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class ImageUploadRequestDto
  {
   [Required(ErrorMessage = "File is required")]
        public IFormFile File { get; set; } = null!;
        
     [StringLength(50, ErrorMessage = "Folder name cannot exceed 50 characters")]
        public string Folder { get; set; } = "products";
    }

    public class MultipleImageUploadRequestDto
    {
     [Required(ErrorMessage = "At least one file is required")]
[MinLength(1, ErrorMessage = "At least one file is required")]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
        
        [StringLength(50, ErrorMessage = "Folder name cannot exceed 50 characters")]
   public string Folder { get; set; } = "products";
    }
}