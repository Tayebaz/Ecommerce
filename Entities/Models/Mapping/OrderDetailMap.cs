using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class OrderDetailMap : EntityTypeConfiguration<OrderDetail>
    {
        public OrderDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.OrderId);

            // Properties
            this.Property(t => t.OrderedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.OrderStatus)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Phone)
                .IsRequired()
                .HasMaxLength(50);

            //this.Property(t => t.MobilePhone)
            //    .HasMaxLength(50);

            //this.Property(t => t.Address)
            //    .HasMaxLength(150);

            //this.Property(t => t.City)
            //    .HasMaxLength(50);

            //this.Property(t => t.Pincode)
            //    .HasMaxLength(50);

            this.Property(t => t.OrderCode)
                .HasMaxLength(20);

            this.Property(t => t.TotalPrice)
                .HasMaxLength(50);

            this.Property(t => t.PaymentType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PaymentId)
                .HasMaxLength(50);

            this.Property(t => t.TokenKey)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("OrderDetails");
            this.Property(t => t.OrderId).HasColumnName("OrderId");
            this.Property(t => t.OrderedBy).HasColumnName("OrderedBy");
            this.Property(t => t.OrderStatus).HasColumnName("OrderStatus");
            this.Property(t => t.OrderDate).HasColumnName("OrderDate");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Phone).HasColumnName("Phone");
            //this.Property(t => t.MobilePhone).HasColumnName("MobilePhone");
            //this.Property(t => t.Address).HasColumnName("Address");
            //this.Property(t => t.City).HasColumnName("City");
            //this.Property(t => t.Pincode).HasColumnName("Pincode");
            //this.Property(t => t.ShippingOrder).HasColumnName("ShippingOrder");
            this.Property(t => t.OrderCode).HasColumnName("OrderCode");
            this.Property(t => t.TotalPrice).HasColumnName("TotalPrice");
            //this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.PaymentType).HasColumnName("PaymentType");
            this.Property(t => t.PaymentId).HasColumnName("PaymentId");
            this.Property(t => t.TokenKey).HasColumnName("TokenKey");
        }
    }
}
