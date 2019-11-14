using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Entities.Models.Mapping
{
    public class CustomerDetailMap : EntityTypeConfiguration<CustomerDetail>
    {
        public CustomerDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.CustomerId);

            // Properties
            this.Property(t => t.EmaiId)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Address)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Contact)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CustomerName)
                .HasMaxLength(50);

            this.Property(t => t.Encid)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CustomerDetails");
            this.Property(t => t.CustomerId).HasColumnName("CustomerId");
            this.Property(t => t.EmaiId).HasColumnName("EmaiId");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Contact).HasColumnName("Contact");
            this.Property(t => t.CustomerName).HasColumnName("CustomerName");
            this.Property(t => t.Encid).HasColumnName("Encid");
            this.Property(t => t.IsBlocked).HasColumnName("IsBlocked");
        }
    }
}
