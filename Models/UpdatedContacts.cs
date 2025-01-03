using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiIntegrationService.Models
{
    public class UpdatedContacts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("Id")]
        public int Id { get; set; }


        [JsonProperty("TaxpayerName")]
        public required string TaxpayerName { get; set; }

        [JsonProperty("TradeName")]
        public required string Company { get; set; }

        [JsonProperty("VATNumber")]
        public required string VatNumber { get; set; }

        [JsonProperty("TIN")]
        public required string TinNumber { get; set; }

        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;

    }
}