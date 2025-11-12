using Project.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Application.Services
{
    public interface IPaymentMethodService
    {
Task<PaymentMethodDto?> GetByIdAsync(string paymentMethodId);
  Task<IEnumerable<PaymentMethodDto>> GetAllAsync();
    Task<IEnumerable<PaymentMethodDto>> GetActiveMethodsAsync();
        Task<IEnumerable<PaymentMethodDto>> GetByTypeAsync(string type);
      Task AddAsync(PaymentMethodCreateDto paymentMethodDto);
        Task UpdateAsync(PaymentMethodUpdateDto paymentMethodDto);
      Task DeleteAsync(string paymentMethodId);
        Task<bool> ExistsAsync(string paymentMethodId);
  Task ActivateAsync(string paymentMethodId);
  Task DeactivateAsync(string paymentMethodId);
        Task<decimal> CalculateProcessingFeeAsync(string paymentMethodId, decimal amount);
        Task<bool> ValidateAmountAsync(string paymentMethodId, decimal amount);
    }
}