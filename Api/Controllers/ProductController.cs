using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Dtos;
using Project.Application.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productService;
        private readonly IImageService _imageService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
   IProductServices productService,
          IImageService imageService,
   ILogger<ProductController> logger)
        {
            _productService = productService;
     _imageService = imageService;
        _logger = logger;
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
  [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,user")]
        public async Task<IActionResult> GetById(int id)
        {
    try
            {
              var product = await _productService.GetByIdAsync(id);
        if (product == null) 
      return NotFound(new { message = "Product not found" });
     
      return Ok(product);
          }
      catch (Exception ex)
         {
    _logger.LogError(ex, "Error getting product with ID: {ProductId}", id);
      return StatusCode(500, new { message = "Internal server error" });
  }
        }

        /// <summary>
        /// Obtiene todos los productos con paginación y búsqueda
      /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrator,user")]
      public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
   {
         try
      {
    var products = await _productService.GetAllAsync(pageNumber, pageSize, searchTerm);
           return Ok(products);
      }
            catch (Exception ex)
 {
     _logger.LogError(ex, "Error getting products");
         return StatusCode(500, new { message = "Internal server error" });
      }
        }

        /// <summary>
        /// Crea un nuevo producto con o sin imagen
/// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromForm] ProductCreateWithImageDto productDto)
        {
            try
     {
    _logger.LogInformation("Creating product: {Name}", productDto.Name);

    string? imageUrl = null;
    string? publicId = null;

        // Si se proporciona imagen, subirla a Cloudinary
         if (productDto.Image != null)
    {
         var imageResult = await _imageService.UploadImageAsync(productDto.Image, "products");

      if (!imageResult.Success)
    {
       return BadRequest(new
               {
         success = false,
           message = "Failed to upload image",
     error = imageResult.ErrorMessage
            });
}

             imageUrl = imageResult.SecureUrl;
   publicId = imageResult.PublicId;
    }

    // Crear el producto
            var createDto = new ProductCreateDto
      {
            Code = productDto.Code,
            Name = productDto.Name,
         Description = productDto.Description ?? string.Empty,
    Price = productDto.Price,
   Stock = productDto.Stock,
        ImageUri = imageUrl ?? string.Empty
       };

            try
       {
      await _productService.AddAsync(createDto);
   _logger.LogInformation("Product created successfully: {Code}", createDto.Code);
             }
                catch (Exception dbEx)
       {
              // Si falla la BD, eliminar imagen subida
  if (!string.IsNullOrEmpty(publicId))
   {
           try
           {
            await _imageService.DeleteImageAsync(publicId);
}
         catch (Exception cleanupEx)
        {
           _logger.LogWarning(cleanupEx, "Failed to cleanup image after database error");
           }
         }

         throw dbEx;
     }

        var response = new
      {
  success = true,
              message = imageUrl != null ? "Product created successfully with image" : "Product created successfully",
        product = new
      {
              code = createDto.Code,
     name = createDto.Name,
           description = createDto.Description,
               price = createDto.Price,
       stock = createDto.Stock,
   imageUrl = imageUrl
         }
          };

         return StatusCode(StatusCodes.Status201Created, response);
       }
        catch (Exception ex)
            {
   _logger.LogError(ex, "Error creating product");
        return StatusCode(500, new
              {
     success = false,
     message = "Error creating product",
          error = ex.Message
    });
       }
  }

        /// <summary>
        /// Actualiza un producto existente
     /// </summary>
   [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateWithImageDto productDto)
        {
            try
  {
    // Verificar que el producto existe
       var existingProduct = await _productService.GetByIdAsync(id);
        if (existingProduct == null)
         {
       return NotFound(new { message = "Product not found" });
            }

            string? newImageUrl = existingProduct.ImageUri;
            string? publicId = null;

                // Si se proporciona nueva imagen
     if (productDto.Image != null)
          {
  var imageResult = await _imageService.UploadImageAsync(productDto.Image, "products");

 if (!imageResult.Success)
        {
          return BadRequest(new { message = "Failed to upload image", error = imageResult.ErrorMessage });
   }

       newImageUrl = imageResult.SecureUrl;
         publicId = imageResult.PublicId;

         // Eliminar imagen anterior si existe
        if (!string.IsNullOrWhiteSpace(existingProduct.ImageUri))
        {
      try
      {
         await _imageService.DeleteImageByUrlAsync(existingProduct.ImageUri);
            }
   catch (Exception ex)
    {
      _logger.LogWarning(ex, "Failed to delete old image");
             }
      }
     }

       // Actualizar el producto
 var updateDto = new ProductUpdateDto
     {
          ProductId = id,
         Code = productDto.Code,
    Name = productDto.Name,
           Description = productDto.Description,
       Price = productDto.Price,
         Stock = productDto.Stock,
      IsActive = productDto.IsActive,
        ImageUri = newImageUrl ?? string.Empty
                };

      await _productService.UpdateAsync(updateDto);

     var response = new
    {
   message = productDto.Image != null ? "Product updated successfully with new image" : "Product updated successfully",
       product = new
   {
  id = id,
     code = updateDto.Code,
             name = updateDto.Name,
    imageUrl = newImageUrl
   }
    };

     return Ok(response);
   }
            catch (Exception ex)
       {
       _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
    return StatusCode(500, new { message = "Internal server error" });
 }
        }

        /// <summary>
  /// Elimina la imagen de un producto
        /// </summary>
        [HttpDelete("{id}/image")]
  [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteProductImage(int id)
  {
 try
         {
    var product = await _productService.GetByIdAsync(id);
     if (product == null)
            {
         return NotFound(new { message = "Product not found" });
  }

         if (string.IsNullOrWhiteSpace(product.ImageUri))
      {
      return BadRequest(new { message = "Product has no image to delete" });
          }

     var result = await _imageService.DeleteImageByUrlAsync(product.ImageUri);

             if (result)
        {
     // Actualizar el producto para remover la ImageUri
   var updateDto = new ProductUpdateDto
           {
  ProductId = id,
    Code = product.Code,
Name = product.Name,
       Description = product.Description,
            Price = product.Price,
                Stock = product.Stock,
                    IsActive = product.IsActive,
 ImageUri = string.Empty
         };

    await _productService.UpdateAsync(updateDto);
      return Ok(new { message = "Product image deleted successfully" });
       }
  else
   {
     return NotFound(new { message = "Image not found or could not be deleted" });
                }
  }
          catch (Exception ex)
            {
              _logger.LogError(ex, "Error deleting product image for product ID: {ProductId}", id);
        return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Elimina un producto
 /// </summary>
 [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
        {
    try
      {
          // Obtener el producto antes de eliminarlo para gestionar la imagen
       var product = await _productService.GetByIdAsync(id);
                if (product != null && !string.IsNullOrWhiteSpace(product.ImageUri))
   {
        try
   {
         await _imageService.DeleteImageByUrlAsync(product.ImageUri);
  _logger.LogInformation("Product image deleted: {ImageUrl}", product.ImageUri);
    }
           catch (Exception ex)
        {
                _logger.LogWarning(ex, "Failed to delete product image: {ImageUrl}", product.ImageUri);
     }
     }

             await _productService.DeleteAsync(id);
   return NoContent();
    }
       catch (Exception ex)
    {
       _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
       return StatusCode(500, new { message = "Internal server error" });
            }
        }
  }
}
