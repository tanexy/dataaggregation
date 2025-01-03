using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class FreshDeskContactWrapper
{

    public long Id { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("address")]
    public string? Address { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("job_title")]
    public string? JobTitle { get; set; }

    [JsonProperty("language")]
    public string? Language { get; set; }

    [JsonProperty("mobile")]
    public string? Mobile { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("phone")]
    public string? Phone { get; set; }

    [JsonProperty("time_zone")]
    public string? TimeZone { get; set; }

    [JsonProperty("twitter_id")]
    public string? TwitterId { get; set; }

    [JsonProperty("facebook_id")]
    public string? FacebookId { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("csat_rating")]
    public string? CsatRating { get; set; }

    [JsonProperty("preferred_source")]
    public string PreferredSource { get; set; } = string.Empty;

    [JsonProperty("company_id")]
    public long? CompanyId { get; set; }

    [JsonProperty("unique_external_id")]
    public string UniqueExternalId { get; set; } = string.Empty;

    [JsonProperty("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("visitor_id")]
    public string VisitorId { get; set; } = string.Empty;
    [JsonProperty("org_contact_id")]
    public string? OrgContactId { get; set; }
}
