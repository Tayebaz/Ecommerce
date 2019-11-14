using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class OrderedItemMap : EntityTypeConfiguration<OrderedItem>
    {
        public OrderedItemMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ProductName)
                .HasMaxLength(50);

            this.Property(t => t.Attributes)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("OrderedItem");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.OrderId).HasColumnName("OrderId");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.OrderQuantity).HasColumnName("OrderQuantity");
            this.Property(t => t.ProductName).HasColumnName("ProductName");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.DiscountPercent).HasColumnName("DiscountPercent");
            this.Property(t => t.Size).HasColumnName("Size");
            this.Property(t => t.Attributes).HasColumnName("Attributes");
        }
    }
}
