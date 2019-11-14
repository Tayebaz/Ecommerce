using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class SliderSettingMap : EntityTypeConfiguration<SliderSetting>
    {
        public SliderSettingMap()
        {
            // Primary Key
            this.HasKey(t => t.SliderId);

            // Properties
            this.Property(t => t.SliderImage)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.TokenKey)
                .HasMaxLength(100);

            this.Property(t => t.Title)
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("SliderSettings");
            this.Property(t => t.SliderId).HasColumnName("SliderId");
            this.Property(t => t.SliderImage).HasColumnName("SliderImage");
            this.Property(t => t.ImageOrder).HasColumnName("ImageOrder");
            this.Property(t => t.TokenKey).HasColumnName("TokenKey");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
