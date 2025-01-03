using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace ApiIntegrationService.Models
{
    public class Chargebee
    {
        [Key] // Marks this property as the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Configures auto-increment
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? VatNumber { get; set; }
        public string? TinNumber { get; set; }
        public bool? Reseller { get; set; }
        public string MRR { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Locale { get; set; }
        public string PiiCleared { get; set; }
        public string Channel { get; set; }
        public string ResourceVersion { get; set; }
        public bool Deleted { get; set; }

        public Address? BillingAddress { get; set; }
    }

    public class Address
    {
        public string Email { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

    }
}
