using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiIntegrationService.Data;
using ApiIntegrationService.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiIntegrationService.Services
{
    public class DataMatchService
    {
        private readonly IServiceProvider serviceProvider;

        public DataMatchService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task ProcessDataMatches()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Fetch data from all three tables
                var freshdeskData = await dbContext.freshdeskcustomerscontacts.ToListAsync();
                var chargebeeData = await dbContext.ChargebeeCustomers.ToListAsync();
                var zimraData = await dbContext.CustomerContacts.ToListAsync();

                var matchedRecords = new List<UpdatedContacts>();
                var unmatchedRecords = new List<UpdatedContacts>();

                foreach (var chargebee in chargebeeData)
                {
                    if (chargebee == null)
                    {
                        Console.WriteLine("Chargebee record is null. Skipping.");
                        continue;
                    }

                    var chargeBeeAddress = chargebee.BillingAddress != null
                        ? $"{chargebee.BillingAddress.Line1} {chargebee.BillingAddress.Line2} {chargebee.BillingAddress.City}"
                        : "Unknown";

                    // Match records from Freshdesk and Zimra
                    var matchedFreshdesk = freshdeskData.FirstOrDefault(f => f?.Email != null && f.Email == chargebee.Email);
                    var matchedZimra = zimraData.FirstOrDefault(z => z?.Email != null && z.Email == chargebee.Email);

                    if (chargebee.Email == null)
                    {
                        Console.WriteLine($"Chargebee record with ID {chargebee.Id} has null email. Skipping.");
                        continue;
                    }

                    // Check matching conditions
                    bool isIdenticalAll = IsIdentical(chargebee, matchedFreshdesk, matchedZimra);
                    bool isIdenticalChargebeeZimra = IsIdentical(chargebee, matchedZimra) && matchedFreshdesk == null;
                    bool isIdenticalFreshdeskZimra = IsIdentical(matchedFreshdesk, matchedZimra) && chargebee == null;
                    bool isIdenticalChargebeeFreshdesk = IsIdentical(chargebee, matchedFreshdesk) && matchedZimra == null;

                    // Create UpdatedContacts record
                    var updatedContacts = new UpdatedContacts
                    {
                        TaxpayerName = matchedZimra?.TaxpayerName ?? chargebee?.Company,
                        VatNumber = matchedZimra?.VatNumber ?? chargebee?.VatNumber ?? "Unknown",
                        TinNumber = matchedZimra?.TinNumber ?? chargebee?.TinNumber ?? "Unknown",
                        Email = matchedZimra?.Email ?? chargebee?.Email ?? matchedFreshdesk?.Email ?? "Unknown",
                        Address = chargeBeeAddress ?? matchedZimra?.Address ?? matchedFreshdesk?.Address ?? "Unknown",
                        PhoneNo = chargebee?.Phone ?? matchedZimra?.PhoneNo ?? matchedFreshdesk?.Phone ?? "Unknown",
                        Company = chargebee?.Company ?? matchedZimra?.TradeName ?? matchedFreshdesk?.Name ?? "Unknown"
                    };

                    // Check for existing record in UpdatedContacts
                    bool recordExists = await dbContext.UpdatedContacts
                        .AnyAsync(uc => uc.Email == updatedContacts.Email);

                    if (recordExists)
                    {
                        Console.WriteLine($"Duplicate record found for email: {updatedContacts.Email}. Skipping.");
                        continue;
                    }

                    // Categorize the record
                    if (isIdenticalAll || isIdenticalChargebeeZimra || isIdenticalFreshdeskZimra || isIdenticalChargebeeFreshdesk)
                    {
                        matchedRecords.Add(updatedContacts);
                    }
                    else
                    {
                        Console.WriteLine($"No matches found for Chargebee email: {chargebee.Email}");
                        unmatchedRecords.Add(updatedContacts);
                    }
                }

                // Save matched records first
                if (matchedRecords.Any())
                {
                    dbContext.UpdatedContacts.AddRange(matchedRecords);
                }

                // Save unmatched records after matched ones
                if (unmatchedRecords.Any())
                {
                    dbContext.UpdatedContacts.AddRange(unmatchedRecords);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private bool IsIdentical(object? record1, object? record2, object? record3)
        {
            if (record1 == null || record2 == null || record3 == null) return false;
            return record1.Equals(record2) && record2.Equals(record3);
        }

        private bool IsIdentical(object? record1, object? record2)
        {
            if (record1 == null || record2 == null) return false;
            return record1.Equals(record2);
        }
    }
}
