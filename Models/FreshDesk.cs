using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIntegrationService.Models
{


  public class Freshdesk
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; } // Changed to long to handle large IDs
    public bool Active { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? JobTitle { get; set; }
    public string? Language { get; set; }
    public string? Mobile { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? TimeZone { get; set; }
    public string? TwitterId { get; set; }
    public string? CustomFields { get; set; } // Kept as object to handle dynamic fields
    public string? FacebookId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Csatrating { get; set; } // Kept as object for possible numeric or null
    public string? PreferredSource { get; set; }
    public long? CompanyId { get; set; }
    public string? OtherCompanies { get; set; } // Array to match the structure
    public string? UniqueExternalId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? VisitorId { get; set; }
    public string? OrgContactId { get; set; }
    public string[]? OtherPhoneNumbers { get; set; }
  }
}
