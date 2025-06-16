using Project.Application.Dtos;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Services
{
    public class ProductService : IProductServices
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : ToProductDto(product);
        }

        public async Task<ProductDto> GetByCodeAsync(string code)
        {
            var product = await _productRepository.GetByCodeAsync(code);
            return product == null ? null : ToProductDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var products = await _productRepository.GetAllAsync(pageNumber, pageSize, searchTerm);
            return products.Select(p => ToProductDto(p));
        }

        public async Task AddAsync(ProductCreateDto productDto)
        {
            var product = new Product
            {
                Code = productDto.Code,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                IsActive = true
            };
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateAsync(ProductUpdateDto productDto)
        {
            var product = new Product
            {
                ProductId = productDto.ProductId,
                Code = productDto.Code,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                IsActive = productDto.IsActive
            };
            await _productRepository.UpdateAsync(product);
        }

        public Task DeleteAsync(int id)
        {
            return _productRepository.DeleteAsync(id);
        }

        public Task<bool> ExistsAsync(string code)
        {
            return _productRepository.ExistsAsync(code);
        }

        public Task<int> CountAsync(string searchTerm = null)
        {
            return _productRepository.CountAsync(searchTerm);
        }

        public async Task<IEnumerable<ProductDto>> GetAvailableProductsAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var products = await _productRepository.GetAvailableProductsAsync(pageNumber, pageSize, searchTerm);
            return products.Select(p => ToProductDto(p));
        }

        public Task<bool> HasStockAsync(int productId, int quantity)
        {
            return _productRepository.HasStockAsync(productId, quantity);
        }

        // Métodos de mapeo manual entre Product y ProductDto
        private ProductDto ToProductDto(Product product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                Code = product.Code,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock
               
            };
        }
    }
}
