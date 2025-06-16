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
            return await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var query = _context.Invoices
                .Include(i => i.InvoiceDetails)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(i => i.UserId.Contains(searchTerm));
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice != null)
            {
                _context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountAsync(string searchTerm = null)
        {
            var query = _context.Invoices.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(i => i.UserId.Contains(searchTerm));
            return await query.CountAsync();
        }

        public async Task<IEnumerable<InvoiceDetail>> GetInvoiceDetailsAsync(int invoiceId)
        {
            return await _context.InvoiceDetails
                .Where(d => d.InvoiceID == invoiceId)
                .ToListAsync();
        }

        public async Task<InvoiceDetail> GetInvoiceDetailAsync(int invoiceId, int productId)
        {
            return await _context.InvoiceDetails
                .FirstOrDefaultAsync(d => d.InvoiceID == invoiceId && d.ProductID == productId);
        }

        public async Task AddProductToInvoiceAsync(int invoiceId, int productId, int quantity)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            var product = await _context.Products.FindAsync(productId);
            if (invoice != null && product != null)
            {
                var detail = new InvoiceDetail
                {
                    InvoiceID = invoiceId,
                    ProductID = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };
                invoice.InvoiceDetails.Add(detail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveProductFromInvoiceAsync(int invoiceId, int productId)
        {
            var detail = await _context.InvoiceDetails
                .FirstOrDefaultAsync(d => d.InvoiceID == invoiceId && d.ProductID == productId);

            if (detail != null)
            {
                _context.InvoiceDetails.Remove(detail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ProductExistsInInvoiceAsync(int invoiceId, int productId)
        {
            return await _context.InvoiceDetails
                .AnyAsync(d => d.InvoiceID == invoiceId && d.ProductID == productId);
        }
    }
}
