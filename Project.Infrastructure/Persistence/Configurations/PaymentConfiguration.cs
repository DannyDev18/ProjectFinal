using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
     {
            builder.ToTable("Payments");

            builder.HasKey(p => p.PaymentId);
builder.Property(p => p.PaymentId)
    .ValueGeneratedOnAdd();

   builder.Property(p => p.InvoiceId)
      .IsRequired();

  builder.Property(p => p.PaymentMethodId)
     .HasMaxLength(50)
  .IsRequired();

   builder.Property(p => p.Amount)
      .HasColumnType("decimal(18,2)")
    .IsRequired();

        builder.Property(p => p.TransactionId)
       .HasMaxLength(200)
  .IsRequired();

   builder.Property(p => p.Status)
 .HasConversion<int>()
   .IsRequired();

  builder.Property(p => p.PaymentDate)
        .IsRequired();

        builder.Property(p => p.ProcessedAt);

    builder.Property(p => p.ProcessorResponse)
  .HasMaxLength(1000);

     builder.Property(p => p.FailureReason)
       .HasMaxLength(500);

    builder.Property(p => p.CreatedAt)
 .IsRequired()
        .HasDefaultValueSql("GETUTCDATE()");

 builder.Property(p => p.UpdatedAt);

   // Relaciones
     builder.HasOne(p => p.Invoice)
   .WithMany()
     .HasForeignKey(p => p.InvoiceId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(p => p.PaymentMethod)
       .WithMany(pm => pm.Payments)
                .HasForeignKey(p => p.PaymentMethodId)
 .OnDelete(DeleteBehavior.Restrict);

 // Índices
     builder.HasIndex(p => p.InvoiceId)
     .HasDatabaseName("IX_Payments_InvoiceId");

         builder.HasIndex(p => p.TransactionId)
              .IsUnique()
     .HasDatabaseName("IX_Payments_TransactionId");

        builder.HasIndex(p => p.Status)
  .HasDatabaseName("IX_Payments_Status");

  builder.HasIndex(p => p.PaymentDate)
     .HasDatabaseName("IX_Payments_PaymentDate");

      // Configuración futura para PostgreSQL (comentada)
    /*
    builder.ToTable("payments");
  builder.Property(p => p.PaymentId).HasColumnName("payment_id");
         builder.Property(p => p.InvoiceId).HasColumnName("invoice_id");
     builder.Property(p => p.PaymentMethodId).HasColumnName("payment_method_id");
    builder.Property(p => p.Amount).HasColumnName("amount");
    builder.Property(p => p.TransactionId).HasColumnName("transaction_id");
     builder.Property(p => p.Status).HasColumnName("status");
            builder.Property(p => p.PaymentDate).HasColumnName("payment_date");
  builder.Property(p => p.ProcessedAt).HasColumnName("processed_at");
    builder.Property(p => p.ProcessorResponse).HasColumnName("processor_response");
builder.Property(p => p.FailureReason).HasColumnName("failure_reason");
 builder.Property(p => p.CreatedAt).HasColumnName("created_at");
builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
       */
        }
    }
}