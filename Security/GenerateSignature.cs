using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiIntegrationService.Data;
using Microsoft.Extensions.Options;

namespace ApiIntegrationService.Security
{
    public class GenerateSignature
    {
        private readonly IOptions<ActiveCustomersSettings>ActiveCustomersSettings;
        public GenerateSignature(IOptions<ActiveCustomersSettings> ActiveCustomersSettings)
        {
          this.ActiveCustomersSettings = ActiveCustomersSettings;
        }
        public  string Generatesignature(string input )
        {
           var secretKey = ActiveCustomersSettings.Value.signaturekey;
           
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("Input and secret key must not be null or empty.");

            using (var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
               // var s = Convert.ToBase64String(hashBytes);
                //Console.WriteLine(s);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}