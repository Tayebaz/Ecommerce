using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class CompareListMap : EntityTypeConfiguration<CompareList>
    {
        public CompareListMap()
        {
            // Primary Key
            this.HasKey(t => t.CompareListId);

            // Properties
            this.Property(t => t.Attributes)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CompareList");
            this.Property(t => t.CompareListId).HasColumnName("CompareListId");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Size).HasColumnName("Size");
            this.Property(t => t.Attributes).HasColumnName("Attributes");
        }
    }
}
