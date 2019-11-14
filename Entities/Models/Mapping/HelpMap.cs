using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class HelpMap : EntityTypeConfiguration<Help>
    {
        public HelpMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.TokenKey)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Contents)
                .IsRequired();

            this.Property(t => t.ContentType)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Help");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TokenKey).HasColumnName("TokenKey");
            this.Property(t => t.Contents).HasColumnName("Contents");
            this.Property(t => t.ContentType).HasColumnName("ContentType");
        }
    }
}
