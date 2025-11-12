using Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Domain.Interfaces
{
    public interface IPaymentMethodRepository : IRepository<PaymentMethod>
    {
        Task<PaymentMethod?> GetByIdAsync(string paymentMethodId);
     Task<IEnumerable<PaymentMethod>> GetActiveMethodsAsync();
  Task<IEnumerable<PaymentMethod>> GetActivePaymentMethodsAsync(); // Para compatibilidad
   Task<IEnumerable<PaymentMethod>> GetByTypeAsync(PaymentType type);
      Task<bool> ExistsByIdAsync(string paymentMethodId);
      Task<bool> ExistsByNameAsync(string name);
      Task<IEnumerable<PaymentMethod>> GetOrderedMethodsAsync();
        Task<IEnumerable<PaymentMethod>> GetOrderedPaymentMethodsAsync(); // Para compatibilidad
     Task<PaymentMethod?> GetByNameAsync(string name);
 Task<decimal> GetProcessingFeeAsync(string paymentMethodId);
      Task<bool> IsValidAmountAsync(string paymentMethodId, decimal amount);
  Task<bool> IsPaymentMethodActiveAsync(string paymentMethodId); // Método faltante
    }
}