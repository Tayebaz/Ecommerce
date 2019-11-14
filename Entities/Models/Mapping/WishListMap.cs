using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class WishListMap : EntityTypeConfiguration<WishList>
    {
        public WishListMap()
        {
            // Primary Key
            this.HasKey(t => t.WishListId);

            // Properties
            this.Property(t => t.Attributes)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WishList");
            this.Property(t => t.WishListId).HasColumnName("WishListId");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Size).HasColumnName("Size");
            this.Property(t => t.Attributes).HasColumnName("Attributes");
        }
    }
}
