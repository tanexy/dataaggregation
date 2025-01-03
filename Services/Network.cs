using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiIntegrationService.Data;
using Microsoft.Extensions.Options;
using ApiIntegrationService.Security;
using System.Text.Json;

namespace ApiIntegrationService.Services
{
    public class Network
    {
        private readonly IOptions<ChargebeeSettings> chargebeeSettings;
        private readonly IOptions<FreshdeskSetttings> freshdeskSettings;
        private readonly IOptions<ActiveCustomersSettings>ActiveCustomersSettings;
       private readonly GenerateSignature generateSignature;

        public Network(GenerateSignature generateSignature, IOptions<ChargebeeSettings> chargebeeSettings, IOptions<FreshdeskSetttings> freshdeskSettings,IOptions<ActiveCustomersSettings> ActiveCustomersSettings )
        {
            this.chargebeeSettings = chargebeeSettings;
            this.freshdeskSettings = freshdeskSettings;
            this.ActiveCustomersSettings = ActiveCustomersSettings;
            this.generateSignature = generateSignature;

        }
        public HttpClient CreateFreshdeskAuthenticatedHttpClient()
        {
            var httpClient = new HttpClient();

            // Set up authentication
            var byteArray = Encoding.ASCII.GetBytes($"{freshdeskSettings.Value.ApiKey}:");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return httpClient;
        }
        public HttpClient CreateSignedHttpClient(string input)
        {
            var httpClient = new HttpClient();
          var url = ActiveCustomersSettings.Value.BaseUrl;
          var signaturekey = ActiveCustomersSettings.Value.signaturekey;
     
           httpClient.DefaultRequestHeaders.Add("x-customerdata-signature", generateSignature.Generatesignature(input));
         
            
        httpClient.BaseAddress = new Uri(url);


            

            return httpClient;
        }
        public HttpClient CreateChargebeeAuthenticatedHttpClient()
        {
            var httpClient = new HttpClient();


            var byteArray = Encoding.ASCII.GetBytes($"{chargebeeSettings.Value.ApiKey}:");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return httpClient;
        }
        public async Task<string> SendfreshdeskGetRequest(AppDbContext dbContext)
        {
            int page = 1;
            using var httpClient = CreateFreshdeskAuthenticatedHttpClient();
            var response = await httpClient.GetAsync($"{freshdeskSettings.Value.BaseUrl}?page={page}");
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            return await response.Content.ReadAsStringAsync();


        }
        public async Task<string> SendchargebeeGetRequest()
        {

            string nextOffset = null; // Start with no offset

            var url = string.IsNullOrEmpty(nextOffset)
                       ? chargebeeSettings.Value.BaseUrl
                       : $"{chargebeeSettings.Value.BaseUrl}?offset={nextOffset}";



            var httpClient = CreateChargebeeAuthenticatedHttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }



    }
    
}