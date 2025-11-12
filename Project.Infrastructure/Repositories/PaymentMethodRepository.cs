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
    public class PaymentMethodRepository : Repository<PaymentMethod>, IPaymentMethodRepository
    {
     public PaymentMethodRepository(ApplicationDBContext context) : base(context)
      {
     }

public async Task<PaymentMethod?> GetByIdAsync(string paymentMethodId)
      {
     return await _dbSet
     .FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethodId);
        }

        public async Task<IEnumerable<PaymentMethod>> GetActiveMethodsAsync()
        {
            return await _dbSet
     .Where(pm => pm.IsActive)
 .OrderBy(pm => pm.DisplayOrder)
        .ThenBy(pm => pm.Name)
       .AsNoTracking()
           .ToListAsync();
        }

    public async Task<IEnumerable<PaymentMethod>> GetActivePaymentMethodsAsync()
     {
      // Alias para GetActiveMethodsAsync para compatibilidad
       return await GetActiveMethodsAsync();
        }

        public async Task<IEnumerable<PaymentMethod>> GetByTypeAsync(PaymentType type)
        {
return await _dbSet
     .Where(pm => pm.Type == type && pm.IsActive)
        .OrderBy(pm => pm.DisplayOrder)
                .ThenBy(pm => pm.Name)
       .AsNoTracking()
    .ToListAsync();
        }

        public async Task<bool> ExistsByIdAsync(string paymentMethodId)
        {
            return await _dbSet
    .AnyAsync(pm => pm.PaymentMethodId == paymentMethodId);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
return await _dbSet
         .AnyAsync(pm => pm.Name.ToLower() == name.ToLower());
        }

    public async Task<IEnumerable<PaymentMethod>> GetOrderedMethodsAsync()
        {
  return await _dbSet
   .Where(pm => pm.IsActive)
   .OrderBy(pm => pm.DisplayOrder)
 .ThenBy(pm => pm.Name)
      .AsNoTracking()
          .ToListAsync();
        }

    public async Task<IEnumerable<PaymentMethod>> GetOrderedPaymentMethodsAsync()
        {
// Alias para GetOrderedMethodsAsync para compatibilidad
            return await GetOrderedMethodsAsync();
        }

        public async Task<PaymentMethod?> GetByNameAsync(string name)
        {
    return await _dbSet
         .FirstOrDefaultAsync(pm => pm.Name.ToLower() == name.ToLower());
        }

   public async Task<decimal> GetProcessingFeeAsync(string paymentMethodId)
        {
 var paymentMethod = await _dbSet
  .AsNoTracking()
       .FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethodId);

            return paymentMethod?.ProcessingFee ?? 0;
        }

        public async Task<bool> IsValidAmountAsync(string paymentMethodId, decimal amount)
        {
          var paymentMethod = await _dbSet
   .AsNoTracking()
     .FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethodId);

  if (paymentMethod == null)
                return false;

         return amount >= paymentMethod.MinAmount && amount <= paymentMethod.MaxAmount;
        }

    public async Task<bool> IsPaymentMethodActiveAsync(string paymentMethodId)
     {
          return await _dbSet
                .AnyAsync(pm => pm.PaymentMethodId == paymentMethodId && pm.IsActive);
        }
    }
}