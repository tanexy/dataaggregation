using System.Numerics;
using Newtonsoft.Json;

public class ChargebeeApiResponse
{
    [JsonProperty("list")]
    public List<ChargebeeContactWrapper>? List { get; set; }
    [JsonProperty("next_offset")]
    public string NextOffset { get; set; }
}


public class ChargebeeContactWrapper
{
    [JsonProperty("customer")]
    public ChargebeeCustomer? CustomerData { get; set; }
}

public class ChargebeeCustomer
{
    //[JsonProperty("id")]
    //  public int Id { get; set; }
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;
    [JsonProperty("company")]
    public string Company { get; set; } = string.Empty;
    [JsonProperty("vat_number")]
    public string VatNumber { get; set; } = string.Empty;
    [JsonProperty("cf_TIN_Number")]
    public string TinNumber { get; set; }
    [JsonProperty("cf_Reseller")]
    public bool Reseller { get; set; }
    [JsonProperty("mrr")]
    public string? MRR { get; set; }

    [JsonProperty("card_status")]
    public string CardStatus { get; set; } = string.Empty;
    [JsonProperty("promotional_credits")]
    public int PromotionalCredits { get; set; }
    [JsonProperty("refundable_credits")]
    public int RefundableCredits { get; set; }
    [JsonProperty("excess_payments")]
    public int ExcessPayments { get; set; }
    [JsonProperty("unbilled_charges")]
    public int UnbilledCharges { get; set; }
    [JsonProperty("preferred_currency_code")]
    public string PreferredCurrencyCode { get; set; } = string.Empty;
    [JsonProperty("deleted")]
    public bool Deleted { get; set; }
    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }
    [JsonProperty("updated_at")]
    public long UpdatedAt { get; set; }
    [JsonProperty("locale")]
    public string Locale { get; set; } = string.Empty;
    [JsonProperty("pii_cleared")]
    public string PiiCleared { get; set; } = string.Empty;
    [JsonProperty("channel")]

    public string Channel { get; set; } = string.Empty;
    [JsonProperty("resource_version")]
    public string ResourceVersion { get; set; } = string.Empty;
    [JsonProperty("billing_address")]

    public ApiIntegrationService.Models.Address? BillingAddress { get; set; }
}
