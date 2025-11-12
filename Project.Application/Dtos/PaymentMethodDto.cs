using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Application.Dtos
{
    public class PaymentMethodDto
    {
        public string PaymentMethodId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
public bool IsActive { get; set; }
        public string Type { get; set; } = string.Empty;
   public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal ProcessingFee { get; set; }
        public string? IconUrl { get; set; }
  public int DisplayOrder { get; set; }
  public DateTime CreatedAt { get; set; }
public DateTime? UpdatedAt { get; set; }
    }

  public class PaymentMethodCreateDto
  {
[Required(ErrorMessage = "Payment method ID is required")]
   [StringLength(50, ErrorMessage = "Payment method ID cannot exceed 50 characters")]
        public string PaymentMethodId { get; set; } = string.Empty;

      [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

   [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
     public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = string.Empty;

 [Range(0, double.MaxValue, ErrorMessage = "Minimum amount cannot be negative")]
    public decimal MinAmount { get; set; }

[Range(0.01, double.MaxValue, ErrorMessage = "Maximum amount must be greater than zero")]
   public decimal MaxAmount { get; set; }

   [Range(0, double.MaxValue, ErrorMessage = "Processing fee cannot be negative")]
   public decimal ProcessingFee { get; set; }

    [Url(ErrorMessage = "Icon URL must be a valid URL")]
      [StringLength(500, ErrorMessage = "Icon URL cannot exceed 500 characters")]
   public string? IconUrl { get; set; }

   [Range(0, int.MaxValue, ErrorMessage = "Display order cannot be negative")]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class PaymentMethodUpdateDto
    {
   [Required(ErrorMessage = "Payment method ID is required")]
   [StringLength(50, ErrorMessage = "Payment method ID cannot exceed 50 characters")]
        public string PaymentMethodId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
   [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
  public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

      [Required(ErrorMessage = "Type is required")]
    public string Type { get; set; } = string.Empty;

[Range(0, double.MaxValue, ErrorMessage = "Minimum amount cannot be negative")]
        public decimal MinAmount { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Maximum amount must be greater than zero")]
   public decimal MaxAmount { get; set; }

   [Range(0, double.MaxValue, ErrorMessage = "Processing fee cannot be negative")]
        public decimal ProcessingFee { get; set; }

     [Url(ErrorMessage = "Icon URL must be a valid URL")]
        [StringLength(500, ErrorMessage = "Icon URL cannot exceed 500 characters")]
        public string? IconUrl { get; set; }

 [Range(0, int.MaxValue, ErrorMessage = "Display order cannot be negative")]
        public int DisplayOrder { get; set; }

 public bool IsActive { get; set; }
    }

 public class PaymentMethodListDto
    {
        public string PaymentMethodId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
 public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public decimal ProcessingFee { get; set; }
 public string? IconUrl { get; set; }
  public int DisplayOrder { get; set; }
    }
}