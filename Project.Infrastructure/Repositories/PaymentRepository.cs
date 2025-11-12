using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Project.Infrastructure.Frameworks.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDBContext context) : base(context)
      {
      }

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceAsync(int invoiceId)
        {
      return await _dbSet
    .Include(p => p.PaymentMethod)
           .Include(p => p.Invoice)
.Where(p => p.InvoiceId == invoiceId)
     .OrderByDescending(p => p.PaymentDate)
          .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserAsync(string userId)
        {
       // Necesitamos acceder a través de las facturas
         return await _dbSet
     .Include(p => p.PaymentMethod)
  .Include(p => p.Invoice)
            .ThenInclude(i => i.Client)
     .Where(p => p.Invoice.UserId == userId)
      .OrderByDescending(p => p.PaymentDate)
          .AsNoTracking()
          .ToListAsync();
        }

   public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
      {
  return await _dbSet
           .Include(p => p.PaymentMethod)
            .Include(p => p.Invoice)
            .Where(p => p.Status == status)
      .OrderByDescending(p => p.PaymentDate)
      .AsNoTracking()
              .ToListAsync();
  }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
    return await _dbSet
 .Include(p => p.PaymentMethod)
    .Include(p => p.Invoice)
        .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
      .OrderByDescending(p => p.PaymentDate)
    .AsNoTracking()
          .ToListAsync();
      }

  public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await _dbSet
         .Include(p => p.PaymentMethod)
            .Include(p => p.Invoice)
             .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<decimal> GetTotalPaymentsAsync(DateTime startDate, DateTime endDate)
      {
        return await _dbSet
  .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate && p.Status == PaymentStatus.Completed)
             .SumAsync(p => p.Amount);
        }

        public async Task<IEnumerable<Payment>> GetFailedPaymentsAsync()
      {
        return await _dbSet
        .Include(p => p.PaymentMethod)
              .Include(p => p.Invoice)
          .Where(p => p.Status == PaymentStatus.Failed)
      .OrderByDescending(p => p.PaymentDate)
        .AsNoTracking()
        .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
        {
      return await _dbSet
   .Include(p => p.PaymentMethod)
  .Include(p => p.Invoice)
    .Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing)
   .OrderBy(p => p.PaymentDate)
        .AsNoTracking()
             .ToListAsync();
        }

        public async Task<bool> HasSuccessfulPaymentAsync(int invoiceId)
        {
            return await _dbSet
                .AnyAsync(p => p.InvoiceId == invoiceId && p.Status == PaymentStatus.Completed);
  }
    }
}