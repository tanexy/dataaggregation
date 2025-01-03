using ApiIntegrationService.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiIntegrationService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Freshdesk> freshdeskcustomerscontacts { get; set; }
        public DbSet<Chargebee> ChargebeeCustomers { get; set; }
        public DbSet<ZimraActiveCustomers> CustomerContacts { get; set; }
        public DbSet<UpdatedContacts> UpdatedContacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Chargebee>()
                .OwnsOne(c => c.BillingAddress, address =>
                {
                    // Explicitly map properties of BillingAddress to custom column names
                    address.Property(a => a.City).HasColumnName("BillingAddressCity");
                    address.Property(a => a.Company).HasColumnName("BillingAddressCompany");
                    address.Property(a => a.Email).HasColumnName("BillingAddressEmail");
                    address.Property(a => a.Phone).HasColumnName("BillingAddressPhone");
                    address.Property(a => a.Line1).HasColumnName("BillingAddressLine1");
                    address.Property(a => a.Line2).HasColumnName("BillingAddressLine2");
                    address.Property(a => a.Line3).HasColumnName("BillingAddressLine3");
                    address.Property(a => a.Country).HasColumnName("BillingAddressCountry");
                });
            modelBuilder.Entity<ZimraActiveCustomers>()
      .HasMany(z => z.Configuration)
      .WithOne(c => c.ZimraActiveCustomers)
            .HasForeignKey(c => c.DeviceId);






        }

    }
}
