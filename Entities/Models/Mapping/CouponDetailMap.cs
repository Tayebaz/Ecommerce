using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class CouponDetailMap : EntityTypeConfiguration<CouponDetail>
    {
        public CouponDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.CouponId);

            // Properties
            this.Property(t => t.CouponCode)
                .HasMaxLength(20);

            this.Property(t => t.ValidFrom)
                .HasMaxLength(15);

            this.Property(t => t.ValidTill)
                .HasMaxLength(15);

            this.Property(t => t.ApplicableOn)
                .HasMaxLength(100);

            this.Property(t => t.NotApplicableOn)
                .HasMaxLength(100);

            this.Property(t => t.TokenKey)
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("CouponDetails");
            this.Property(t => t.CouponId).HasColumnName("CouponId");
            this.Property(t => t.CouponCode).HasColumnName("CouponCode");
            this.Property(t => t.DiscountVal).HasColumnName("DiscountVal");
            this.Property(t => t.ValidFrom).HasColumnName("ValidFrom");
            this.Property(t => t.ValidTill).HasColumnName("ValidTill");
            this.Property(t => t.ApplicableOn).HasColumnName("ApplicableOn");
            this.Property(t => t.NotApplicableOn).HasColumnName("NotApplicableOn");
            this.Property(t => t.TokenKey).HasColumnName("TokenKey");
        }
    }
}
