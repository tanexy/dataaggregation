using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIntegrationService.Models
{
    public class CustomerContacts
    {
        public int Id { get; set; }


        public string TaxPayerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TaxPayerTIN { get; set; } = string.Empty;
        public string VatNumber { get; set; } = string.Empty;
        public string DeviceSerialNo { get; set; } = string.Empty;
        public string DeviceBranchName { get; set; } = string.Empty;
        public DeviceBranchAddress? DeviceBranchAddress { get; set; }
        public deviceBranchContacts? deviceBranchContacts { get; set; }
        public string DeviceOperatingMode { get; set; } = string.Empty;


    }







}