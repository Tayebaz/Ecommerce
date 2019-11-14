using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class WorkingHoursMap : EntityTypeConfiguration<workingHours>
    {
        public WorkingHoursMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("WorkingHours");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.OpenTime).HasColumnName("OpenTime");
            this.Property(t => t.CloseTime).HasColumnName("CloseTime");
            this.Property(t => t.isClosedAllDay).HasColumnName("isClosedAllDay");
        }

    }
}
