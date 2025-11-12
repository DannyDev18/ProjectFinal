using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.Domain.Entities
{
    public class PaymentMethod
    {
        [Key]
public string PaymentMethodId { get; set; } = string.Empty;
        
      [Required]
  [MaxLength(100)]
     public string Name { get; set; } = string.Empty;
        
   [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public bool IsActive { get; set; }
        
        public PaymentType Type { get; set; }
        
    // JSON config for payment processor
    public string? ProcessorConfig { get; set; }
        
        public decimal MinAmount { get; set; }
        
  public decimal MaxAmount { get; set; }
   
        public decimal ProcessingFee { get; set; }
        
        // Para la UI móvil
        public string? IconUrl { get; set; }
    
        // Para ordenar en la UI
        public int DisplayOrder { get; set; }
        
  public DateTime CreatedAt { get; set; }
  
     public DateTime? UpdatedAt { get; set; }

    // Navigation properties
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();

        public PaymentMethod()
        {
            IsActive = true;
       CreatedAt = DateTime.UtcNow;
        }

        public PaymentMethod(string paymentMethodId, string name, PaymentType type) : this()
        {
            if (string.IsNullOrWhiteSpace(paymentMethodId))
        throw new ArgumentException("Payment method ID is required", nameof(paymentMethodId));
 
            if (string.IsNullOrWhiteSpace(name))
throw new ArgumentException("Name is required", nameof(name));

         PaymentMethodId = paymentMethodId;
    Name = name;
            Type = type;
        }

  public void Update(string name, string description, PaymentType type, bool isActive)
{
            if (string.IsNullOrWhiteSpace(name))
           throw new ArgumentException("Name is required", nameof(name));

        Name = name;
    Description = description ?? string.Empty;
      Type = type;
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetProcessingFee(decimal fee)
        {
            if (fee < 0)
        throw new ArgumentException("Processing fee cannot be negative", nameof(fee));
  
            ProcessingFee = fee;
    UpdatedAt = DateTime.UtcNow;
        }

     public void SetAmountLimits(decimal minAmount, decimal maxAmount)
        {
            if (minAmount < 0)
             throw new ArgumentException("Minimum amount cannot be negative", nameof(minAmount));
          
         if (maxAmount <= minAmount)
        throw new ArgumentException("Maximum amount must be greater than minimum amount", nameof(maxAmount));

      MinAmount = minAmount;
 MaxAmount = maxAmount;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum PaymentStatus
    {
Pending = 0,
     Processing = 1,
   Completed = 2,
        Failed = 3,
        Cancelled = 4,
        Refunded = 5
 }

    public enum PaymentType
    {
 Cash = 0,  // Efectivo
        CreditCard = 1,      // Tarjeta de Crédito
        DebitCard = 2,         // Tarjeta de Débito
        BankTransfer = 3,      // Transferencia Bancaria
        PayPal = 4,         // PayPal
        Stripe = 5,            // Stripe
        MobileMoney = 6,       // Dinero móvil
        Cryptocurrency = 7,    // Criptomonedas
      GiftCard = 8  // Tarjeta de regalo
    }
}