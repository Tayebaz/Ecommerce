using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class AddToCartMap : EntityTypeConfiguration<AddToCart>
    {
        public AddToCartMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Attributes)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AddToCart");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Attributes).HasColumnName("Attributes");
            this.Property(t => t.Size).HasColumnName("Size");
        }
    }
}
