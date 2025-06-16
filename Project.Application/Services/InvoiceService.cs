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
    public class InvoiceService : IInvoiceServices
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IProductRepository _productRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository, IProductRepository productRepository)
        {
            _invoiceRepository = invoiceRepository;
            _productRepository = productRepository;
        }

        public async Task<InvoiceDto> GetByIdAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            return invoice == null ? null : ToInvoiceDto(invoice);
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var invoices = await _invoiceRepository.GetAllAsync(pageNumber, pageSize, searchTerm);
            return invoices.Select(i => ToInvoiceDto(i));
        }

        public async Task AddAsync(InvoiceCreateDto invoiceDto)
        {
            var invoice = new Invoice
            {
                InvoiceNumber = invoiceDto.InvoiceNumber,
                ClientId = invoiceDto.ClientId,
                UserId = invoiceDto.UserId,
                IssueDate = invoiceDto.IssueDate ?? DateTime.UtcNow,
                Observations = invoiceDto.Observations,
                InvoiceDetails = invoiceDto.InvoiceDetails?.Select(d => new InvoiceDetail
                {
                    ProductID = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Subtotal = d.Quantity * d.UnitPrice
                }).ToList()
            };

            // Calcula totales
            invoice.Subtotal = invoice.InvoiceDetails?.Sum(d => d.Subtotal) ?? 0;
            invoice.Tax = invoice.Subtotal * 0.12m;
            invoice.Total = invoice.Subtotal + invoice.Tax;

            await _invoiceRepository.AddAsync(invoice);
        }

        public async Task UpdateAsync(InvoiceUpdateDto invoiceDto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceDto.InvoiceId);
            if (invoice == null)
                throw new InvalidOperationException("Invoice not found");

            invoice.InvoiceNumber = invoiceDto.InvoiceNumber;
            invoice.ClientId = invoiceDto.ClientId;
            invoice.UserId = invoiceDto.UserId;
            invoice.IssueDate = invoiceDto.IssueDate;
            invoice.Observations = invoiceDto.Observations;

            // Actualiza detalles
            invoice.InvoiceDetails = invoiceDto.InvoiceDetails?.Select(d => new InvoiceDetail
            {
                InvoiceDetailId = d.InvoiceDetailId,
                ProductID = d.ProductId,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Quantity * d.UnitPrice,
                InvoiceID = invoiceDto.InvoiceId
            }).ToList();

            // Recalcula totales
            invoice.Subtotal = invoice.InvoiceDetails?.Sum(d => d.Subtotal) ?? 0;
            invoice.Tax = invoice.Subtotal * 0.12m;
            invoice.Total = invoice.Subtotal + invoice.Tax;

            await _invoiceRepository.UpdateAsync(invoice);
        }

        public Task DeleteAsync(int id)
        {
            return _invoiceRepository.DeleteAsync(id);
        }

        public Task<int> CountAsync(string searchTerm = null)
        {
            return _invoiceRepository.CountAsync(searchTerm);
        }

        public async Task<IEnumerable<InvoiceDetailDto>> GetInvoiceDetailsAsync(int invoiceId)
        {
            var details = await _invoiceRepository.GetInvoiceDetailsAsync(invoiceId);
            return details.Select(d => ToInvoiceDetailDto(d));
        }

        public async Task AddProductToInvoiceAsync(int invoiceId, int productId, int quantity)
        {
            var exists = await _invoiceRepository.ProductExistsInInvoiceAsync(invoiceId, productId);
            if (exists)
                throw new InvalidOperationException("El producto ya existe en la factura.");

            var hasStock = await _productRepository.HasStockAsync(productId, quantity);
            if (!hasStock)
                throw new InvalidOperationException("No hay suficiente stock para el producto.");

            await _invoiceRepository.AddProductToInvoiceAsync(invoiceId, productId, quantity);
            await _productRepository.DecreaseStockAsync(productId, quantity);
            await CalculateTotalsAsync(invoiceId);
        }

        public async Task RemoveProductFromInvoiceAsync(int invoiceId, int productId)
        {
            var detail = await _invoiceRepository.GetInvoiceDetailAsync(invoiceId, productId);
            if (detail == null)
                throw new InvalidOperationException("El producto no existe en la factura.");

            await _invoiceRepository.RemoveProductFromInvoiceAsync(invoiceId, productId);
            await _productRepository.IncreaseStockAsync(productId, detail.Quantity);
            await CalculateTotalsAsync(invoiceId);
        }

        public async Task UpdateProductInInvoiceAsync(int invoiceId, int oldProductId, int newProductId, int newQuantity)
        {
            await RemoveProductFromInvoiceAsync(invoiceId, oldProductId);
            await AddProductToInvoiceAsync(invoiceId, newProductId, newQuantity);
        }

        public Task<bool> ProductExistsInInvoiceAsync(int invoiceId, int productId)
        {
            return _invoiceRepository.ProductExistsInInvoiceAsync(invoiceId, productId);
        }

        public async Task<bool> ValidateStockForInvoiceAsync(int invoiceId)
        {
            var details = await _invoiceRepository.GetInvoiceDetailsAsync(invoiceId);
            foreach (var detail in details)
            {
                var hasStock = await _productRepository.HasStockAsync(detail.ProductID, detail.Quantity);
                if (!hasStock)
                    return false;
            }
            return true;
        }

        public async Task<InvoiceDto> CalculateTotalsAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            var details = await _invoiceRepository.GetInvoiceDetailsAsync(invoiceId);

            decimal subtotal = details.Sum(d => d.Quantity * d.UnitPrice);
            decimal iva = subtotal * 0.12m;
            decimal total = subtotal + iva;

            invoice.Subtotal = subtotal;
            invoice.Tax = iva;
            invoice.Total = total;

            await _invoiceRepository.UpdateAsync(invoice);

            return ToInvoiceDto(invoice);
        }

        // Métodos de mapeo manual
        private InvoiceDto ToInvoiceDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                ClientId = invoice.ClientId,
                UserId = invoice.UserId,
                IssueDate = invoice.IssueDate,
                Subtotal = invoice.Subtotal,
                Tax = invoice.Tax,
                Total = invoice.Total,
                Observations = invoice.Observations,
                Client = invoice.Client == null ? null : new ClientDto
                {
                    ClientId = invoice.Client.ClientId,
                    FirstName = invoice.Client.FirstName
                },
                InvoiceDetails = invoice.InvoiceDetails?.Select(ToInvoiceDetailDto).ToList()
            };
        }

        private InvoiceDetailDto ToInvoiceDetailDto(InvoiceDetail d)
        {
            return new InvoiceDetailDto
            {
                InvoiceDetailId = d.InvoiceDetailId,
                InvoiceId = d.InvoiceID,
                ProductId = d.ProductID,
                ProductName = d.Product?.Name,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Subtotal
            };
        }
    }
}
