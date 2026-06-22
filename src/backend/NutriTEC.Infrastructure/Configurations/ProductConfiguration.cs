using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;
using NutriTEC.Domain.Enums;

namespace NutriTEC.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> entity)
    {
        // This mapping follows the product table and constraints defined by the SQL Server schema.
        entity.ToTable("product");
        entity.HasKey(product => product.BarCode).HasName("pk_product");

        entity.Property(product => product.BarCode).HasColumnName("bar_code").HasMaxLength(40).IsUnicode(false);
        entity.Property(product => product.PortionUnit).HasColumnName("portion_unit").HasMaxLength(30).IsUnicode(false);
        entity.Property(product => product.Sodium).HasColumnName("sodium").HasColumnType("numeric(10, 2)");
        entity.Property(product => product.ProductStatus)
            .HasColumnName("product_status")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasConversion(
                status => status.ToDatabaseValue(),
                value => ProductStatusExtensions.FromDatabaseValue(value));
        entity.Property(product => product.Iron).HasColumnName("iron").HasColumnType("numeric(10, 2)");
        entity.Property(product => product.Calcium).HasColumnName("calcium").HasColumnType("numeric(10, 2)");
        entity.Property(product => product.Vitamins).HasColumnName("vitamins").HasMaxLength(120).IsUnicode(false);
        entity.Property(product => product.PortionSize).HasColumnName("portion_size").HasColumnType("numeric(10, 2)");
        entity.Property(product => product.Calories).HasColumnName("calories").HasColumnType("numeric(10, 2)");
        entity.Property(product => product.Protein).HasColumnName("protein").HasColumnType("numeric(10, 2)");
        entity.Property(product => product.Carbohydrates).HasColumnName("carbohydrates").HasColumnType("numeric(10, 2)");
        entity.Property(product => product.Fat).HasColumnName("fat").HasColumnType("numeric(10, 2)");
        entity.Property(product => product.ProductName).HasColumnName("product_name").HasMaxLength(120).IsUnicode(false);
        entity.Property(product => product.UserId).HasColumnName("user_id");

        entity.HasOne(product => product.User)
            .WithMany()
            .HasForeignKey(product => product.UserId)
            .HasConstraintName("fk_product_app_user")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
