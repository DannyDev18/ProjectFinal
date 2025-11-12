using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Persistence.Configurations
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethods");

   builder.HasKey(pm => pm.PaymentMethodId);
         builder.Property(pm => pm.PaymentMethodId)
   .HasMaxLength(50)
   .IsRequired();

  builder.Property(pm => pm.Name)
    .HasMaxLength(100)
      .IsRequired();

   builder.Property(pm => pm.Description)
          .HasMaxLength(500);

       builder.Property(pm => pm.IsActive)
 .IsRequired()
    .HasDefaultValue(true);

     builder.Property(pm => pm.Type)
         .HasConversion<int>()
     .IsRequired();

  builder.Property(pm => pm.ProcessorConfig)
  .HasColumnType("nvarchar(max)");

   builder.Property(pm => pm.MinAmount)
     .HasColumnType("decimal(18,2)")
         .HasDefaultValue(0);

        builder.Property(pm => pm.MaxAmount)
     .HasColumnType("decimal(18,2)")
      .HasDefaultValue(999999.99m);

  builder.Property(pm => pm.ProcessingFee)
 .HasColumnType("decimal(18,2)")
     .HasDefaultValue(0);

            builder.Property(pm => pm.IconUrl)
     .HasMaxLength(500);

            builder.Property(pm => pm.DisplayOrder)
    .HasDefaultValue(0);

    builder.Property(pm => pm.CreatedAt)
        .IsRequired()
     .HasDefaultValueSql("GETUTCDATE()");

   builder.Property(pm => pm.UpdatedAt);

     // Índices
     builder.HasIndex(pm => pm.Name)
         .IsUnique();

    builder.HasIndex(pm => pm.Type);

      builder.HasIndex(pm => pm.IsActive);

        builder.HasIndex(pm => pm.DisplayOrder);

       // Relaciones
builder.HasMany(pm => pm.Payments)
      .WithOne(p => p.PaymentMethod)
   .HasForeignKey(p => p.PaymentMethodId)
     .OnDelete(DeleteBehavior.Restrict);

      // Configuración futura para PostgreSQL (comentada)
      /*
            builder.ToTable("payment_methods");
      builder.Property(pm => pm.PaymentMethodId).HasColumnName("payment_method_id");
       builder.Property(pm => pm.Name).HasColumnName("name");
       builder.Property(pm => pm.Description).HasColumnName("description");
            builder.Property(pm => pm.IsActive).HasColumnName("is_active");
          builder.Property(pm => pm.Type).HasColumnName("type");
   builder.Property(pm => pm.ProcessorConfig).HasColumnName("processor_config");
        builder.Property(pm => pm.MinAmount).HasColumnName("min_amount");
      builder.Property(pm => pm.MaxAmount).HasColumnName("max_amount");
builder.Property(pm => pm.ProcessingFee).HasColumnName("processing_fee");
 builder.Property(pm => pm.IconUrl).HasColumnName("icon_url");
       builder.Property(pm => pm.DisplayOrder).HasColumnName("display_order");
         builder.Property(pm => pm.CreatedAt).HasColumnName("created_at");
        builder.Property(pm => pm.UpdatedAt).HasColumnName("updated_at");
       */
   }
    }
}