using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Application.Dtos
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
   public string PaymentMethodId { get; set; } = string.Empty;
    public string PaymentMethodName { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public string TransactionId { get; set; } = string.Empty;
 public string Status { get; set; } = string.Empty;
 public DateTime PaymentDate { get; set; }
      public DateTime? ProcessedAt { get; set; }
   public string? ProcessorResponse { get; set; }
        public string? FailureReason { get; set; }
   public DateTime CreatedAt { get; set; }
 public DateTime? UpdatedAt { get; set; }
    }

    public class PaymentCreateDto
    {
   [Required(ErrorMessage = "Invoice ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invoice ID must be greater than zero")]
public int InvoiceId { get; set; }
        
[Required(ErrorMessage = "Payment method ID is required")]
   [StringLength(50, ErrorMessage = "Payment method ID cannot exceed 50 characters")]
   public string PaymentMethodId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Amount is required")]
 [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
 
     [Required(ErrorMessage = "Transaction ID is required")]
        [StringLength(200, ErrorMessage = "Transaction ID cannot exceed 200 characters")]
     public string TransactionId { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Processor response cannot exceed 1000 characters")]
   public string? ProcessorResponse { get; set; }
    }

    public class PaymentUpdateDto
    {
     [Required(ErrorMessage = "Payment ID is required")]
 [Range(1, int.MaxValue, ErrorMessage = "Payment ID must be greater than zero")]
        public int PaymentId { get; set; }
   
   [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; } = string.Empty;
        
 [StringLength(1000, ErrorMessage = "Processor response cannot exceed 1000 characters")]
   public string? ProcessorResponse { get; set; }
      
        [StringLength(500, ErrorMessage = "Failure reason cannot exceed 500 characters")]
   public string? FailureReason { get; set; }
    }

 public class PaymentStatusDto
 {
  public int PaymentId { get; set; }
      public string Status { get; set; } = string.Empty;
  public DateTime? ProcessedAt { get; set; }
   public string? ProcessorResponse { get; set; }
       public string? FailureReason { get; set; }
  }

    // DTOs adicionales para PaymentController
    public class CancelPaymentRequestDto
    {
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }
    }

    public class ValidateAmountRequestDto
{
        [Required(ErrorMessage = "Invoice ID is required")]
      [Range(1, int.MaxValue, ErrorMessage = "Invoice ID must be greater than zero")]
      public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
   [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
    }
}