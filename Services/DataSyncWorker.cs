using System.Text;
using System.Text.Json;
using ApiIntegrationService.Data;
using ApiIntegrationService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiIntegrationService.Services
{
    public class DataSyncWorker : BackgroundService
    {
        private readonly Network network;
        private readonly IServiceProvider serviceProvider;
        private readonly ChargebeeSettings chargebeeSettings;
        private readonly DataMatchService dataMatchService;

        private readonly FreshdeskSetttings freshdeskSettings;

        public DataSyncWorker(IServiceProvider serviceProvider, IOptions<ChargebeeSettings> chargebeeSettings, IOptions<FreshdeskSetttings> freshdeskSettings, Network network, DataMatchService dataMatchService)
        {
            this.serviceProvider = serviceProvider;
            this.chargebeeSettings = chargebeeSettings.Value;
            this.freshdeskSettings = freshdeskSettings.Value;
            this.dataMatchService = dataMatchService;

            this.network = network;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();


                        await SyncFreshdeskData(dbContext);
                        await SyncChargebeeData(dbContext);
                        await SyncCustomerData(dbContext);
                    }


                    await dataMatchService.ProcessDataMatches();
                }
                catch (Exception ex)
                {
                    // Handle exceptions and log if necessary
                    Console.WriteLine($"An error occurred in DataSyncWorker: {ex.Message}");
                }

                // Delay before the next execution cycle
                await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
            }
        }

        private async Task SyncFreshdeskData(AppDbContext dbContext)
        {
            using var httpClient = network.CreateFreshdeskAuthenticatedHttpClient();
            // Pagination parameters
            int page = 1;

            try
            {
                List<FreshDeskContactWrapper> freshdeskContacts = new List<FreshDeskContactWrapper>();

                // Loop through all pages
                while (true)
                {
                    var response = await httpClient.GetAsync($"{freshdeskSettings.BaseUrl}?page={page}");
                    response.EnsureSuccessStatusCode();

                    var responseData = await response.Content.ReadAsStringAsync();

                    var contactsOnPage = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FreshDeskContactWrapper>>(responseData);

                    if (contactsOnPage == null || contactsOnPage.Count == 0)
                    {
                        break;
                    }

                    freshdeskContacts.AddRange(contactsOnPage); // Add this page's data to the list

                    page++;
                }

                // Process the retrieved contacts and save to the database
                foreach (var customer in freshdeskContacts)
                {
                    if (customer != null)
                    {
                        var record = new Freshdesk
                        {
                            Active = customer.Active,
                            Address = customer.Address,
                            Description = customer.Description,
                            Email = customer.Email,
                            JobTitle = customer.JobTitle,
                            Language = customer.Language,
                            Mobile = customer.Mobile,
                            Name = customer.Name,
                            Phone = customer.Phone,
                            TimeZone = customer.TimeZone,
                            TwitterId = customer.TwitterId,
                            FacebookId = customer.FacebookId,
                            CreatedAt = customer.CreatedAt,
                            UpdatedAt = customer.UpdatedAt,
                            Csatrating = customer.CsatRating,
                            PreferredSource = customer.PreferredSource,
                            CompanyId = customer.CompanyId,
                            UniqueExternalId = customer.UniqueExternalId,
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            VisitorId = customer.VisitorId,
                            OrgContactId = customer.OrgContactId,
                        };



                        // Check if the record already exists
                        var existing = await dbContext.freshdeskcustomerscontacts
                            .FirstOrDefaultAsync(c => c.Email == record.Email); // Match by Email or another unique field

                        if (existing != null)
                        {
                            bool hasChanges = false;


                            if (existing.Phone != record.Phone) { existing.Phone = record.Phone; hasChanges = true; }
                            if (existing.Mobile != record.Mobile) { existing.Mobile = record.Mobile; hasChanges = true; }
                            if (existing.Name != record.Name) { existing.Name = record.Name; hasChanges = true; }
                            if (existing.Description != record.Description) { existing.Description = record.Description; hasChanges = true; }
                            if (existing.Address != record.Address) { existing.Address = record.Address; hasChanges = true; }
                            if (existing.JobTitle != record.JobTitle) { existing.JobTitle = record.JobTitle; hasChanges = true; }
                            if (existing.Language != record.Language) { existing.Language = record.Language; hasChanges = true; }
                            if (existing.TimeZone != record.TimeZone) { existing.TimeZone = record.TimeZone; hasChanges = true; }
                            if (existing.TwitterId != record.TwitterId) { existing.TwitterId = record.TwitterId; hasChanges = true; }
                            if (existing.FacebookId != record.FacebookId) { existing.FacebookId = record.FacebookId; hasChanges = true; }
                            if (existing.CreatedAt != record.CreatedAt) { existing.CreatedAt = record.CreatedAt; hasChanges = true; }
                            if (existing.UpdatedAt != record.UpdatedAt) { existing.UpdatedAt = record.UpdatedAt; hasChanges = true; }
                            if (existing.Csatrating != record.Csatrating) { existing.Csatrating = record.Csatrating; hasChanges = true; }
                            if (existing.PreferredSource != record.PreferredSource) { existing.PreferredSource = record.PreferredSource; hasChanges = true; }
                            if (existing.CompanyId != record.CompanyId) { existing.CompanyId = record.CompanyId; hasChanges = true; }
                            if (existing.UniqueExternalId != record.UniqueExternalId) { existing.UniqueExternalId = record.UniqueExternalId; hasChanges = true; }
                            if (existing.FirstName != record.FirstName) { existing.FirstName = record.FirstName; hasChanges = true; }
                            if (existing.LastName != record.LastName) { existing.LastName = record.LastName; hasChanges = true; }
                            if (existing.VisitorId != record.VisitorId) { existing.VisitorId = record.VisitorId; hasChanges = true; }
                            if (existing.OrgContactId != record.OrgContactId) { existing.OrgContactId = record.OrgContactId; hasChanges = true; }

                            /*  if (hasChanges)
                              {
                                  Console.WriteLine("Updating record For: " + existing.FirstName);

                              }
                              // Update existing record
                              // dbContext.Entry(existing).CurrentValues.SetValues(record);*/

                        }
                        else
                        {
                            // Add new record
                            await dbContext.freshdeskcustomerscontacts.AddAsync(record);

                        }
                    }
                }

                // Save all changes to the database
                await dbContext.SaveChangesAsync();
                Console.WriteLine("All records have been saved successfully.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while syncing Freshdesk data: {ex.Message}");
            }
        }

        private async Task SyncChargebeeData(AppDbContext dbContext)
        {
            using var httpClient = network.CreateChargebeeAuthenticatedHttpClient();




            string nextOffset = null; // Start with no offset
            bool hasMore = true;      // Flag to track pagination

            try
            {
                while (hasMore)
                {
                    //  API URL with optional offset
                    var url = string.IsNullOrEmpty(nextOffset)
                        ? chargebeeSettings.BaseUrl
                        : $"{chargebeeSettings.BaseUrl}?offset={nextOffset}";


                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var responseData = await response.Content.ReadAsStringAsync();

                    var chargebeeResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ChargebeeApiResponse>(responseData);

                    if (chargebeeResponse?.List != null)
                    {
                        foreach (var chargebeeWrapper in chargebeeResponse.List)
                        {
                            var customer = chargebeeWrapper.CustomerData;
                            if (customer != null)
                            {
                                var record = new Chargebee
                                {
                                    Email = customer.Email,
                                    Phone = customer.Phone,
                                    Company = customer.Company,
                                    VatNumber = customer.VatNumber,
                                    TinNumber = customer.TinNumber,
                                    Reseller = customer.Reseller,
                                    MRR = customer.MRR,
                                    Deleted = customer.Deleted,
                                    CreatedAt = DateTimeOffset.FromUnixTimeSeconds(customer.CreatedAt).UtcDateTime,
                                    UpdatedAt = DateTimeOffset.FromUnixTimeSeconds(customer.UpdatedAt).UtcDateTime,
                                    Locale = customer.Locale,
                                    PiiCleared = customer.PiiCleared,
                                    Channel = customer.Channel,
                                    ResourceVersion = customer.ResourceVersion,
                                    BillingAddress = customer.BillingAddress != null
                                        ? new Address
                                        {
                                            Email = customer.BillingAddress.Email,
                                            Company = customer.BillingAddress.Company,
                                            Phone = customer.BillingAddress.Phone,
                                            Line1 = customer.BillingAddress.Line1,
                                            Line2 = customer.BillingAddress.Line2,
                                            Line3 = customer.BillingAddress.Line3,
                                            City = customer.BillingAddress.City,
                                            Country = customer.BillingAddress.Country,
                                        }
                                        : null
                                };


                                var existing = await dbContext.ChargebeeCustomers
                                    .Include(c => c.BillingAddress)
                                    .FirstOrDefaultAsync(c => c.Email == record.Email);
                                if (existing != null)
                                {
                                    bool hasChanges = false;


                                    if (existing.Phone != record.Phone) { existing.Phone = record.Phone; hasChanges = true; }
                                    if (existing.Company != record.Company) { existing.Company = record.Company; hasChanges = true; }
                                    if (existing.VatNumber != record.VatNumber) { existing.VatNumber = record.VatNumber; hasChanges = true; }
                                    if (existing.TinNumber != record.TinNumber) { existing.TinNumber = record.TinNumber; hasChanges = true; }
                                    if (existing.Reseller != record.Reseller) { existing.Reseller = record.Reseller; hasChanges = true; }
                                    if (existing.Deleted != record.Deleted) { existing.Deleted = record.Deleted; hasChanges = true; }
                                    if (existing.MRR != record.MRR) { existing.MRR = record.MRR; hasChanges = true; }
                                    if (existing.Locale != record.Locale) { existing.Locale = record.Locale; hasChanges = true; }
                                    if (existing.PiiCleared != record.PiiCleared) { existing.PiiCleared = record.PiiCleared; hasChanges = true; }
                                    if (existing.Channel != record.Channel) { existing.Channel = record.Channel; hasChanges = true; }
                                    if (existing.ResourceVersion != record.ResourceVersion) { existing.ResourceVersion = record.ResourceVersion; hasChanges = true; }

                                    // Compare nested BillingAddress
                                    if (record.BillingAddress != null)
                                    {
                                        if (existing.BillingAddress == null)
                                        {
                                            existing.BillingAddress = record.BillingAddress;
                                            hasChanges = true;
                                        }
                                        else
                                        {
                                            if (existing.BillingAddress.Email != record.BillingAddress.Email) { existing.BillingAddress.Email = record.BillingAddress.Email; hasChanges = true; }
                                            if (existing.BillingAddress.Company != record.BillingAddress.Company) { existing.BillingAddress.Company = record.BillingAddress.Company; hasChanges = true; }
                                            if (existing.BillingAddress.Phone != record.BillingAddress.Phone) { existing.BillingAddress.Phone = record.BillingAddress.Phone; hasChanges = true; }
                                            if (existing.BillingAddress.Line1 != record.BillingAddress.Line1) { existing.BillingAddress.Line1 = record.BillingAddress.Line1; hasChanges = true; }
                                            if (existing.BillingAddress.Line2 != record.BillingAddress.Line2) { existing.BillingAddress.Line2 = record.BillingAddress.Line2; hasChanges = true; }
                                            if (existing.BillingAddress.Line3 != record.BillingAddress.Line3) { existing.BillingAddress.Line3 = record.BillingAddress.Line3; hasChanges = true; }
                                            if (existing.BillingAddress.City != record.BillingAddress.City) { existing.BillingAddress.City = record.BillingAddress.City; hasChanges = true; }
                                            if (existing.BillingAddress.Country != record.BillingAddress.Country) { existing.BillingAddress.Country = record.BillingAddress.Country; hasChanges = true; }
                                        }
                                    }


                                    if (hasChanges)
                                    {
                                        //Console.WriteLine("Updating record for: " + existing.Email);
                                    }
                                }
                                else
                                {

                                    await dbContext.ChargebeeCustomers.AddAsync(record);
                                    //Console.WriteLine("Adding new record for : " + record.Email);
                                }
                            }
                        }

                        // Save changes after processing the page
                        await dbContext.SaveChangesAsync();
                    }


                    nextOffset = chargebeeResponse?.NextOffset;
                    hasMore = !string.IsNullOrEmpty(nextOffset);
                    if (nextOffset == null)
                    {
                        Console.WriteLine("No more data to sync");
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while syncing Chargebee data: {ex.Message}");
            }
        }
        public async Task SyncCustomerData(AppDbContext dbContext)
        {
            try
            {
                // Define date range
                var dateFrom = new DateTime(2023, 08, 30, 22, 14, 23);
                var dateTo = DateTime.Now.AddDays(1);

                // Serialize date range input
                var input = JsonSerializer.Serialize(new ZimraCustomerDateRange
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo
                });

                var content = new StringContent(input, Encoding.UTF8, "application/json");

                // Create a signed HTTP client
                var httpClient = network.CreateSignedHttpClient(input);

                // Send POST request
                var response = await httpClient.PostAsync("/ExportCustomerDetails/sync?=", content);

                response.EnsureSuccessStatusCode();

                // Parse response
                var responseData = await response.Content.ReadAsStringAsync();


                // Deserialize response data into ZimraActiveCustomersWrapper array
                var customerDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<ZimraActiveCustomersWrapper[]>(responseData);

                foreach (var customer in customerDetails)
                {
                    // Deserialize CurrentConfig if present
                    if (!string.IsNullOrEmpty(customer.CurrentConfig))
                    {


                        var deserializedConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrentConfig>(customer.CurrentConfig);


                        customer.Configuration = new List<CurrentConfig> { deserializedConfig };


                        customer.Email = deserializedConfig?.DeviceBranchContacts.Email;
                        customer.Address = deserializedConfig?.DeviceBranchAddress.HouseNo + " " + deserializedConfig?.DeviceBranchAddress.Street + " " + deserializedConfig?.DeviceBranchAddress.City;

                        customer.PhoneNo = deserializedConfig?.DeviceBranchContacts.PhoneNo;
                    }

                    // Check if customer exists in the database
                    var existingCustomer = await dbContext.CustomerContacts
                        .FirstOrDefaultAsync(c => c.DeviceId == customer.DeviceId);

                    if (existingCustomer != null)
                    {
                        // Update existing customer if there are changes
                        bool isUpdated = false;

                        if (existingCustomer.VatNumber != customer.VatNumber)
                        {
                            existingCustomer.VatNumber = customer.VatNumber;
                            isUpdated = true;
                        }
                        if (existingCustomer.Email != customer.Email)
                        {
                            existingCustomer.Email = customer.Email;
                            isUpdated = true;
                        }
                        if (existingCustomer.PhoneNo != customer.PhoneNo)
                        {
                            existingCustomer.PhoneNo = customer.PhoneNo;
                            isUpdated = true;
                        }
                        if (existingCustomer.Address != customer.Address)
                        {
                            existingCustomer.Address = customer.Address;
                            isUpdated = true;
                        }

                        if (existingCustomer.TinNumber != customer.TinNumber)
                        {
                            existingCustomer.TinNumber = customer.TinNumber;
                            isUpdated = true;
                        }

                        if (existingCustomer.ModelName != customer.ModelName)
                        {
                            existingCustomer.ModelName = customer.ModelName;
                            isUpdated = true;
                        }

                        if (existingCustomer.TaxpayerName != customer.TaxpayerName)
                        {
                            existingCustomer.TaxpayerName = customer.TaxpayerName;
                            isUpdated = true;
                        }

                        if (existingCustomer.BPNumber != customer.BPNumber)
                        {
                            existingCustomer.BPNumber = customer.BPNumber;
                            isUpdated = true;
                        }

                        if (existingCustomer.ModelVersion != customer.ModelVersion)
                        {
                            existingCustomer.ModelVersion = customer.ModelVersion;
                            isUpdated = true;
                        }

                        if (existingCustomer.SerialNo != customer.SerialNo)
                        {
                            existingCustomer.SerialNo = customer.SerialNo;
                            isUpdated = true;
                        }

                        if (existingCustomer.TradeName != customer.TradeName)
                        {
                            existingCustomer.TradeName = customer.TradeName;
                            isUpdated = true;
                        }

                        if (existingCustomer.IsActive != customer.IsActive)
                        {
                            existingCustomer.IsActive = customer.IsActive;
                            isUpdated = true;
                        }

                        if (existingCustomer.ActivatedOn != customer.ActivatedOn)
                        {
                            existingCustomer.ActivatedOn = customer.ActivatedOn;
                            isUpdated = true;
                        }

                        if (existingCustomer.NextPingTime != customer.NextPingTime)
                        {
                            existingCustomer.NextPingTime = customer.NextPingTime;
                            isUpdated = true;
                        }

                        if (isUpdated)
                        {
                            dbContext.CustomerContacts.Update(existingCustomer);
                        }
                    }
                    else
                    {
                        // Create a new customer if not found in the database
                        var zimraCustomer = new ZimraActiveCustomers
                        {
                            DeviceId = customer.DeviceId,
                            PhoneNo = customer.PhoneNo,
                            Address = customer.Address,
                            VatNumber = customer.VatNumber,
                            TinNumber = customer.TinNumber,
                            Email = customer.Email,
                            ModelName = customer.ModelName,
                            TaxpayerName = customer.TaxpayerName,
                            BPNumber = customer.BPNumber,
                            ModelVersion = customer.ModelVersion,
                            SerialNo = customer.SerialNo,
                            TradeName = customer.TradeName,
                            IsActive = customer.IsActive,
                            ActivatedOn = customer.ActivatedOn,
                            NextPingTime = customer.NextPingTime,

                        };

                        await dbContext.CustomerContacts.AddAsync(zimraCustomer);
                    }
                }

                // Save changes to the database
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Customer data synced successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

    }
}