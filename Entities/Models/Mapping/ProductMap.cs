using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            // Primary Key
            this.HasKey(t => t.ProductID);

            // Properties
            this.Property(t => t.TokenKey)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ProductName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.ProductCode)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.ShortDescription)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("Product");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.TokenKey).HasColumnName("TokenKey");
            this.Property(t => t.ProductName).HasColumnName("ProductName");
            this.Property(t => t.ProductCode).HasColumnName("ProductCode");
            this.Property(t => t.BrandId).HasColumnName("BrandId");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.SubCategoryId).HasColumnName("SubCategoryId");
            this.Property(t => t.DiscountPercent).HasColumnName("DiscountPercent");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ShortDescription).HasColumnName("ShortDescription");
            this.Property(t => t.Availability).HasColumnName("Availability");
            this.Property(t => t.Price).HasColumnName("Price");
        }
    }
}
