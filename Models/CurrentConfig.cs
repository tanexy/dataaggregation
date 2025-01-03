using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIntegrationService.Models
{
  public class DeviceBranchAddress
  {
    [Key]
    public int Id { get; set; }
    public string Province { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string HouseNo { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
  }

  public class deviceBranchContacts
  {
    [Key] // Marks this property as the primary key
    public int Id { get; set; } = 0;
    public string PhoneNo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
  }


  public class CurrentConfig
  {

    [Key]
    public string Id { get; set; }

    public int DeviceId { get; set; }
    public ZimraActiveCustomers? ZimraActiveCustomers { get; set; }
    public string OperationID { get; set; } = string.Empty;
    public string TaxPayerName { get; set; } = string.Empty;
    public string TaxPayerTIN { get; set; } = string.Empty;
    public string VatNumber { get; set; } = string.Empty;
    // public string DeviceSerialNo { get; set; }
    public string DeviceBranchName { get; set; } = string.Empty;
    public DeviceBranchAddress? DeviceBranchAddress { get; set; }
    public deviceBranchContacts? DeviceBranchContacts { get; set; }


  }

}