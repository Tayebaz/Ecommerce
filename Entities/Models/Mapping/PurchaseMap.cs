using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class PurchaseMap : EntityTypeConfiguration<Purchase>
    {
        public PurchaseMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Quantity)
                .IsFixedLength()
                .HasMaxLength(10);

            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.PurchaseDate)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Purchase");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Productid).HasColumnName("Productid");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.TotalPrice).HasColumnName("TotalPrice");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.PurchaseDate).HasColumnName("PurchaseDate");
        }
    }
}
