using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ApiIntegrationService.Models
{
    public class ZimraActiveCustomers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DeviceId { get; set; }

        [Required]
        [Column(TypeName = "longtext")]
        public string SerialNo { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "longtext")]
        public string ModelName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "longtext")]
        public string ModelVersion { get; set; } = string.Empty;
        [Column(TypeName = "longtext")]
        public string? Email { get; set; }



        [Column(TypeName = "longtext")]
        public string? BPNumber { get; set; }

        [Required]
        [Column(TypeName = "longtext")]
        public string TaxpayerName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "longtext")]
        public string TradeName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "longtext")]
        public string VatNumber { get; set; } = string.Empty;



        [Required]
        public bool IsActive { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? PhoneNo { get; set; } = string.Empty;

        public DateTime? ActivatedOn { get; set; }

        public DateTime? NextPingTime { get; set; }


        [Required]
        public bool IsLive { get; set; } = false;

        [Required]
        public int FiscalProviderId { get; set; }


        [Required]
        public bool IsSwitchedOn { get; set; } = true;







        public string? TinNumber { get; set; }

        public List<CurrentConfig> Configuration { get; set; } = new List<CurrentConfig>();


    }

}
