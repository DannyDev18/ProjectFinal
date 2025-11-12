// MÓDULO DE PAGOS - PREPARADO PARA IMPLEMENTACIÓN FUTURA
// Descomentar cuando sea necesario implementar sistema de pagos

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Entities;

namespace Project.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        
        [Required]
        public int InvoiceId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string PaymentMethodId { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
   
        [Required]
        [MaxLength(200)]
        public string TransactionId { get; set; } = string.Empty;
        
        [Required]
        public PaymentStatus Status { get; set; }
     
        public DateTime PaymentDate { get; set; }
        
        public DateTime? ProcessedAt { get; set; }
    
        [MaxLength(1000)]
        public string? ProcessorResponse { get; set; }
        
        [MaxLength(500)]
        public string? FailureReason { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(InvoiceId))]
        public virtual Invoice Invoice { get; set; } = null!;
        
        [ForeignKey(nameof(PaymentMethodId))]
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;

        public Payment()
        {
            PaymentDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            Status = PaymentStatus.Pending;
        }

        public Payment(int invoiceId, string paymentMethodId, decimal amount, string transactionId) : this()
        {
            if (invoiceId <= 0)
                throw new ArgumentException("Invoice ID must be greater than zero", nameof(invoiceId));

            if (string.IsNullOrWhiteSpace(paymentMethodId))
                throw new ArgumentException("Payment method ID is required", nameof(paymentMethodId));
       
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));
        
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Transaction ID is required", nameof(transactionId));

            InvoiceId = invoiceId;
            PaymentMethodId = paymentMethodId;
            Amount = amount;
            TransactionId = transactionId;
        }

        public void MarkAsCompleted(string? processorResponse = null)
        {
            Status = PaymentStatus.Completed;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            ProcessorResponse = processorResponse;
            FailureReason = null;
        }

        public void MarkAsFailed(string failureReason)
        {
            if (string.IsNullOrWhiteSpace(failureReason))
                throw new ArgumentException("Failure reason is required", nameof(failureReason));

            Status = PaymentStatus.Failed;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            FailureReason = failureReason;
        }

        public void MarkAsRefunded()
        {
            Status = PaymentStatus.Refunded;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsCancelled()
        {
            Status = PaymentStatus.Cancelled;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void StartProcessing()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be processed");

            Status = PaymentStatus.Processing;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsCompleted => Status == PaymentStatus.Completed;
        public bool IsFailed => Status == PaymentStatus.Failed;
        public bool IsPending => Status == PaymentStatus.Pending;
        public bool IsProcessing => Status == PaymentStatus.Processing;
    }
}