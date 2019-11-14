using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class OrderListMap : EntityTypeConfiguration<OrderList>
    {
        public OrderListMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id, t.OrderCode, t.CustomerName, t.Address, t.City, t.Pincode, t.Contact, t.OrderDate });

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.OrderCode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CustomerName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Address)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.City)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Pincode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Contact)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.OrderDate)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.OrderStatus)
                .HasMaxLength(50);

            this.Property(t => t.DeliveryDate)
                .HasMaxLength(50);

            this.Property(t => t.OrderQuantity)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("OrderList");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OrderCode).HasColumnName("OrderCode");
            this.Property(t => t.CustomerName).HasColumnName("CustomerName");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.Pincode).HasColumnName("Pincode");
            this.Property(t => t.Contact).HasColumnName("Contact");
            this.Property(t => t.OrderDate).HasColumnName("OrderDate");
            this.Property(t => t.OrderStatus).HasColumnName("OrderStatus");
            this.Property(t => t.DeliveryDate).HasColumnName("DeliveryDate");
            this.Property(t => t.OrderQuantity).HasColumnName("OrderQuantity");
        }
    }
}
