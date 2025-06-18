using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Project.Infrastructure.Frameworks.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Project.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDBContext _context;

        public InvoiceRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Invoice> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invoice ID must be greater than zero.", nameof(id));
            return await _context.Invoices
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Product)
                .Include(i => i.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            if (pageNumber <= 0) throw new ArgumentException("Page number must be greater than zero.", nameof(pageNumber));
            if (pageSize <= 0) throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

            var query = _context.Invoices
                .Include(i => i.InvoiceDetails)
                 .ThenInclude(d => d.Product)
                .Include(i => i.Client)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(i =>
                    i.UserId.ToLower().Contains(term) ||
                    i.InvoiceNumber.ToLower().Contains(term) ||
                    i.Client.FirstName.ToLower().Contains(term) ||
                    i.Client.LastName.ToLower().Contains(term)
                );
            }

            return await query
                .OrderByDescending(i => i.IssueDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Invoice invoice)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            var existing = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId);

            if (existing == null)
                throw new InvalidOperationException("Invoice does not exist.");

            // Actualiza campos relevantes
            existing.InvoiceNumber = invoice.InvoiceNumber;
            existing.ClientId = invoice.ClientId;
            existing.UserId = invoice.UserId;
            existing.IssueDate = invoice.IssueDate;
            existing.Observations = invoice.Observations;
            existing.Subtotal = invoice.Subtotal;
            existing.Tax = invoice.Tax;
            existing.Total = invoice.Total;

            // Actualizar detalles de la factura:
            _context.InvoiceDetails.RemoveRange(existing.InvoiceDetails);
            foreach (var detail in invoice.InvoiceDetails)
            {
                detail.InvoiceID = existing.InvoiceId;
                detail.Invoice = null; // MUY IMPORTANTE: evita doble tracking
                existing.InvoiceDetails.Add(detail);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invoice ID must be greater than zero.", nameof(id));
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
                throw new InvalidOperationException("Invoice does not exist.");

            _context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync(string searchTerm = null)
        {
            var query = _context.Invoices
             .Include(i => i.Client)
             .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(i =>
                    i.UserId.ToLower().Contains(term) ||
                    i.InvoiceNumber.ToLower().Contains(term) ||
                    i.Client.FirstName.ToLower().Contains(term) ||
                    i.Client.LastName.ToLower().Contains(term)
                );
            }
            return await query.CountAsync();
        }

        public async Task<IEnumerable<InvoiceDetail>> GetInvoiceDetailsAsync(int invoiceId)
        {
            return await _context.InvoiceDetails
             .Where(d => d.InvoiceID == invoiceId)
             .Include(d => d.Product)
             .AsNoTracking()
             .ToListAsync();
        }

        public async Task<InvoiceDetail> GetInvoiceDetailAsync(int invoiceId, int productId)
        {
            return await _context.InvoiceDetails
            .Include(d => d.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.InvoiceID == invoiceId && d.ProductID == productId);
        }

        public async Task AddProductToInvoiceAsync(int invoiceId, int productId, int quantity)
        {
            var invoice = await _context.Invoices
            .Include(i => i.InvoiceDetails)
            .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            var product = await _context.Products.FindAsync(productId);
            if (invoice == null) throw new InvalidOperationException("Invoice does not exist.");
            if (product == null) throw new InvalidOperationException("Product does not exist.");

            var detail = new InvoiceDetail
            {
                InvoiceID = invoiceId,
                ProductID = productId,
                Quantity = quantity,
                UnitPrice = product.Price,
                Subtotal = product.Price * quantity
            };

            invoice.InvoiceDetails.Add(detail);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveProductFromInvoiceAsync(int invoiceId, int productId)
        {
            var detail = await _context.InvoiceDetails
           .FirstOrDefaultAsync(d => d.InvoiceID == invoiceId && d.ProductID == productId);

            if (detail == null)
                throw new InvalidOperationException("Product does not exist in invoice.");

            _context.InvoiceDetails.Remove(detail);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ProductExistsInInvoiceAsync(int invoiceId, int productId)
        {
            return await _context.InvoiceDetails
                .AnyAsync(d => d.InvoiceID == invoiceId && d.ProductID == productId);
        }
    }
}
