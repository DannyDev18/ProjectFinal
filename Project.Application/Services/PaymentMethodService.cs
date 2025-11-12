using Project.Application.Dtos;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Application.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IUnitOfWork _unitOfWork;

  public PaymentMethodService(IUnitOfWork unitOfWork)
        {
   _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
}

     public async Task<PaymentMethodDto?> GetByIdAsync(string paymentMethodId)
        {
     if (string.IsNullOrWhiteSpace(paymentMethodId))
        throw new ArgumentException("Payment method ID is required.", nameof(paymentMethodId));

            var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(paymentMethodId.Trim());
    return paymentMethod != null ? ToPaymentMethodDto(paymentMethod) : null;
        }

      public async Task<IEnumerable<PaymentMethodDto>> GetAllAsync()
  {
            var paymentMethods = await _unitOfWork.PaymentMethods.GetAllAsync();
  return paymentMethods.Select(ToPaymentMethodDto).OrderBy(p => p.DisplayOrder);
}

        public async Task<IEnumerable<PaymentMethodDto>> GetActiveMethodsAsync()
   {
  var activeMethods = await _unitOfWork.PaymentMethods.GetActiveMethodsAsync();
    return activeMethods.Select(ToPaymentMethodDto).OrderBy(p => p.DisplayOrder);
      }

        public async Task<IEnumerable<PaymentMethodDto>> GetByTypeAsync(string type)
     {
      if (string.IsNullOrWhiteSpace(type))
throw new ArgumentException("Payment type is required.", nameof(type));

      if (!Enum.TryParse<PaymentType>(type, true, out var paymentType))
   throw new ArgumentException("Invalid payment type.", nameof(type));

        var methods = await _unitOfWork.PaymentMethods.GetByTypeAsync(paymentType);
   return methods.Select(ToPaymentMethodDto).OrderBy(p => p.DisplayOrder);
        }

        public async Task AddAsync(PaymentMethodCreateDto paymentMethodDto)
   {
     if (paymentMethodDto == null)
throw new ArgumentNullException(nameof(paymentMethodDto));

        if (string.IsNullOrWhiteSpace(paymentMethodDto.PaymentMethodId))
       throw new ArgumentException("Payment method ID is required.", nameof(paymentMethodDto.PaymentMethodId));

       if (string.IsNullOrWhiteSpace(paymentMethodDto.Name))
       throw new ArgumentException("Name is required.", nameof(paymentMethodDto.Name));

            if (paymentMethodDto.MaxAmount <= paymentMethodDto.MinAmount)
   throw new ArgumentException("Maximum amount must be greater than minimum amount.");

  // Check if payment method ID already exists
  if (await _unitOfWork.PaymentMethods.ExistsByIdAsync(paymentMethodDto.PaymentMethodId.Trim()))
       throw new InvalidOperationException("A payment method with this ID already exists.");

          if (!Enum.TryParse<PaymentType>(paymentMethodDto.Type, true, out var paymentType))
        throw new ArgumentException("Invalid payment type.", nameof(paymentMethodDto.Type));

       var paymentMethod = new PaymentMethod(
                paymentMethodDto.PaymentMethodId.Trim(),
   paymentMethodDto.Name.Trim(),
                paymentType)
         {
    Description = paymentMethodDto.Description?.Trim() ?? string.Empty,
           MinAmount = paymentMethodDto.MinAmount,
   MaxAmount = paymentMethodDto.MaxAmount,
   ProcessingFee = paymentMethodDto.ProcessingFee,
      IconUrl = paymentMethodDto.IconUrl?.Trim(),
   DisplayOrder = paymentMethodDto.DisplayOrder,
   IsActive = paymentMethodDto.IsActive
      };

      await _unitOfWork.PaymentMethods.AddAsync(paymentMethod);
  await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(PaymentMethodUpdateDto paymentMethodDto)
  {
       if (paymentMethodDto == null)
      throw new ArgumentNullException(nameof(paymentMethodDto));

            var existingMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(paymentMethodDto.PaymentMethodId);
      if (existingMethod == null)
throw new InvalidOperationException("Payment method does not exist.");

     if (!Enum.TryParse<PaymentType>(paymentMethodDto.Type, true, out var paymentType))
       throw new ArgumentException("Invalid payment type.", nameof(paymentMethodDto.Type));

  existingMethod.Update(
         paymentMethodDto.Name.Trim(),
  paymentMethodDto.Description?.Trim() ?? string.Empty,
 paymentType,
  paymentMethodDto.IsActive);

 existingMethod.SetAmountLimits(paymentMethodDto.MinAmount, paymentMethodDto.MaxAmount);
            existingMethod.SetProcessingFee(paymentMethodDto.ProcessingFee);

          _unitOfWork.PaymentMethods.Update(existingMethod);
  await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(string paymentMethodId)
  {
   if (string.IsNullOrWhiteSpace(paymentMethodId))
          throw new ArgumentException("Payment method ID is required.", nameof(paymentMethodId));

     var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(paymentMethodId.Trim());
            if (paymentMethod == null)
         throw new InvalidOperationException("Payment method does not exist.");

            _unitOfWork.PaymentMethods.Remove(paymentMethod);
         await _unitOfWork.SaveChangesAsync();
  }

   public async Task<bool> ExistsAsync(string paymentMethodId)
        {
  if (string.IsNullOrWhiteSpace(paymentMethodId))
  throw new ArgumentException("Payment method ID is required.", nameof(paymentMethodId));

  return await _unitOfWork.PaymentMethods.ExistsByIdAsync(paymentMethodId.Trim());
     }

        public async Task ActivateAsync(string paymentMethodId)
        {
   var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(paymentMethodId);
            if (paymentMethod == null)
    throw new InvalidOperationException("Payment method does not exist.");

     paymentMethod.Update(paymentMethod.Name, paymentMethod.Description, paymentMethod.Type, true);
            _unitOfWork.PaymentMethods.Update(paymentMethod);
  await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(string paymentMethodId)
  {
   var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(paymentMethodId);
     if (paymentMethod == null)
                throw new InvalidOperationException("Payment method does not exist.");

   paymentMethod.Update(paymentMethod.Name, paymentMethod.Description, paymentMethod.Type, false);
 _unitOfWork.PaymentMethods.Update(paymentMethod);
            await _unitOfWork.SaveChangesAsync();
     }

public async Task<decimal> CalculateProcessingFeeAsync(string paymentMethodId, decimal amount)
        {
    var processingFee = await _unitOfWork.PaymentMethods.GetProcessingFeeAsync(paymentMethodId);
    return amount * (processingFee / 100);
        }

  public async Task<bool> ValidateAmountAsync(string paymentMethodId, decimal amount)
        {
     return await _unitOfWork.PaymentMethods.IsValidAmountAsync(paymentMethodId, amount);
        }

        // Mapping method
      private PaymentMethodDto ToPaymentMethodDto(PaymentMethod paymentMethod)
    {
            if (paymentMethod == null) return null!;

   return new PaymentMethodDto
   {
PaymentMethodId = paymentMethod.PaymentMethodId,
  Name = paymentMethod.Name,
      Description = paymentMethod.Description,
         IsActive = paymentMethod.IsActive,
   Type = paymentMethod.Type.ToString(),
      MinAmount = paymentMethod.MinAmount,
    MaxAmount = paymentMethod.MaxAmount,
      ProcessingFee = paymentMethod.ProcessingFee,
             IconUrl = paymentMethod.IconUrl,
  DisplayOrder = paymentMethod.DisplayOrder,
      CreatedAt = paymentMethod.CreatedAt,
       UpdatedAt = paymentMethod.UpdatedAt
            };
 }
    }
}