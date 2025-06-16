using Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetAllAsync(int pageNumber, int pageSize, string searchTerm = null);
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(int id);
        Task<int> CountAsync(string searchTerm = null);

        // Métodos para detalle de factura (detalle)
        Task<IEnumerable<InvoiceDetail>> GetInvoiceDetailsAsync(int invoiceId);
        Task<InvoiceDetail> GetInvoiceDetailAsync(int invoiceId, int productId);

        // Lógica de ventas (maestro-detalle)
        Task AddProductToInvoiceAsync(int invoiceId, int productId, int quantity);
        Task RemoveProductFromInvoiceAsync(int invoiceId, int productId);
        Task<bool> ProductExistsInInvoiceAsync(int invoiceId, int productId);
    }
}
