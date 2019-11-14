using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class BrandMap : EntityTypeConfiguration<Brand>
    {
        public BrandMap()
        {
            // Primary Key
            this.HasKey(t => t.BrandId);

            // Properties
            this.Property(t => t.TokenKey)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.BrandName)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Brand");
            this.Property(t => t.BrandId).HasColumnName("BrandId");
            this.Property(t => t.TokenKey).HasColumnName("TokenKey");
            this.Property(t => t.BrandName).HasColumnName("BrandName");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");

            // Relationships
            this.HasRequired(t => t.Category)
                .WithMany(t => t.Brands)
                .HasForeignKey(d => d.CategoryId);

        }
    }
}
