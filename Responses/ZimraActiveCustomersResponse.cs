using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ApiIntegrationService.Models;
using Newtonsoft.Json;

public class ZimraActiveCustomersWrapper
{

    [Key]
    [JsonProperty("Id")]
    public int Id { get; set; }

    [JsonProperty("DeviceId")]
    public int DeviceId { get; set; }

    [JsonProperty("SerialNo")]
    public required string SerialNo { get; set; }

    [JsonProperty("ModelName")]
    public required string ModelName { get; set; }

    [JsonProperty("ModelVersion")]
    public required string ModelVersion { get; set; }



    [JsonProperty("BPNumber")]
    public string? BPNumber { get; set; }
    [JsonProperty("TaxpayerName")]
    public required string TaxpayerName { get; set; }

    [JsonProperty("TradeName")]
    public required string TradeName { get; set; }

    [JsonProperty("VATNumber")]
    public required string VatNumber { get; set; }

    [JsonProperty("TIN")]
    public required string TinNumber { get; set; }
    [JsonProperty("IsActive")]
    public bool IsActive { get; set; }
    [JsonProperty("ActivatedOn")]
    public DateTime? ActivatedOn { get; set; }
    public DateTime? NextPingTime { get; set; }
    public string Address { get; set; } = string.Empty;
    [JsonProperty("IsLive")]
    public bool IsLive { get; set; } = false;

    [JsonProperty("FiscalProviderId")]
    public int FiscalProviderId { get; set; }

    [JsonProperty("IsSwitchedOn")]
    public bool IsSwitchedOn { get; set; } = true;


    public string Email { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public List<CurrentConfig> Configuration { get; set; } = new List<CurrentConfig>();

    [JsonProperty("CurrentConfig")]
    public string CurrentConfig { get; set; } = string.Empty;

}

