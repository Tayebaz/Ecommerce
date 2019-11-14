using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class SubCategoryMap : EntityTypeConfiguration<SubCategory>
    {
        public SubCategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.SubCategoryId);

            // Properties
            this.Property(t => t.TokenKey)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.SubCategoryName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SubCategory");
            this.Property(t => t.SubCategoryId).HasColumnName("SubCategoryId");
            this.Property(t => t.TokenKey).HasColumnName("TokenKey");
            this.Property(t => t.SubCategoryName).HasColumnName("SubCategoryName");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");

            // Relationships
            this.HasRequired(t => t.Category)
                .WithMany(t => t.SubCategories)
                .HasForeignKey(d => d.CategoryId);

        }
    }
}
