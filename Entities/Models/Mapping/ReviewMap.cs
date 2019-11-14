using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class ReviewMap : EntityTypeConfiguration<Review>
    {
        public ReviewMap()
        {
            // Primary Key
            this.HasKey(t => t.ReviewId);

            // Properties
            this.Property(t => t.Review1)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Review");
            this.Property(t => t.ReviewId).HasColumnName("ReviewId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Review1).HasColumnName("Review");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.Rating).HasColumnName("Rating");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
        }
    }
}
