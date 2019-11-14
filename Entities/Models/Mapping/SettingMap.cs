using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class SettingMap : EntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Logo)
                .HasMaxLength(50);

            this.Property(t => t.CompanyName)
                .HasMaxLength(50);

            this.Property(t => t.PageTitle)
                .HasMaxLength(50);

            this.Property(t => t.PageKeyword)
                .HasMaxLength(50);

            this.Property(t => t.PageDescription)
                .HasMaxLength(100);

            this.Property(t => t.FacebookLink)
                .HasMaxLength(50);

            this.Property(t => t.TwitterLink)
                .HasMaxLength(50);

            this.Property(t => t.GoogleLink)
                .HasMaxLength(50);

            this.Property(t => t.Address)
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Phone)
                .HasMaxLength(50);

            this.Property(t => t.WebAddress)
                .HasMaxLength(50);

            this.Property(t => t.Mon)
                .HasMaxLength(50);

            this.Property(t => t.Tue)
                .HasMaxLength(50);

            this.Property(t => t.Wed)
                .HasMaxLength(50);

            this.Property(t => t.Thur)
                .HasMaxLength(50);

            this.Property(t => t.Fri)
                .HasMaxLength(50);

            this.Property(t => t.Sat)
                .HasMaxLength(50);

            this.Property(t => t.Sun)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Setting");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Logo).HasColumnName("Logo");
            this.Property(t => t.CompanyName).HasColumnName("CompanyName");
            this.Property(t => t.PageTitle).HasColumnName("PageTitle");
            this.Property(t => t.PageKeyword).HasColumnName("PageKeyword");
            this.Property(t => t.PageDescription).HasColumnName("PageDescription");
            this.Property(t => t.FacebookLink).HasColumnName("FacebookLink");
            this.Property(t => t.TwitterLink).HasColumnName("TwitterLink");
            this.Property(t => t.GoogleLink).HasColumnName("GoogleLink");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.WebAddress).HasColumnName("WebAddress");
            this.Property(t => t.Mon).HasColumnName("Mon");
            this.Property(t => t.Tue).HasColumnName("Tue");
            this.Property(t => t.Wed).HasColumnName("Wed");
            this.Property(t => t.Thur).HasColumnName("Thur");
            this.Property(t => t.Fri).HasColumnName("Fri");
            this.Property(t => t.Sat).HasColumnName("Sat");
            this.Property(t => t.Sun).HasColumnName("Sun");
            this.Property(t => t.isClosed).HasColumnName("isClosed");
        }
    }
}
