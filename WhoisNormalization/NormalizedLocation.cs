// -----------------------------------------------------------------------
// <copyright file="NormalizedLocation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Parsers;

    public class NormalizedLocation
    {
        private static HashSet<string> addressFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Address",
            "Org-Address",
            "Customer Address",
            "customer-address",
            "Address-1"
        };

        private static HashSet<string> streetFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Street",
            "Street-Address"
        };

        private static HashSet<string> geolocationFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "GeoLocation"
        };

        private static HashSet<string> cityFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "City",
            "Org-City",
            "Organization-City",
            "Customer City"
        };

        private static HashSet<string> stateProvinceFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "State",
            "Org-State",
            "State-Province",
            "StateProv",
            "Organization-State",
            "Customer State/Province",
            "State/Prov"
        };

        private static HashSet<string> postalCodeFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Postal-Code",
            "Org-Zip",
            "Customer Postal Code",
            "Organization-Zip",
            "PostalCode"
        };

        private static HashSet<string> countryFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Country",
            "Country-Code",
            "Org-Country",
            "Organization-Country",
            "Customer Country Code"
        };

        public string Address { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string StateProvince { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Geolocation { get; set; }

        public static NormalizedLocation TryParseFromSection(RawWhoisSection section)
        {
            var location = new NormalizedLocation()
            {
                Address = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, addressFields),
                Street = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, streetFields),
                City = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, cityFields),
                StateProvince = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, stateProvinceFields),
                PostalCode = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, postalCodeFields),
                Country = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, countryFields),
                Geolocation = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, geolocationFields)
            };

            return location;
        }

        public bool AddressSeemsValid()
        {
            if (!string.IsNullOrWhiteSpace(this.Address))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(this.Country))
            {
                if (!string.IsNullOrWhiteSpace(this.City))
                {
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(this.PostalCode))
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            var ret = new StringBuilder();

            NormalizationUtils.AddToBuilderWithComma(ret, this.Address);
            NormalizationUtils.AddToBuilderWithComma(ret, this.Street);
            NormalizationUtils.AddToBuilderWithComma(ret, this.City);
            NormalizationUtils.AddToBuilderWithComma(ret, this.StateProvince);
            NormalizationUtils.AddToBuilderWithComma(ret, this.PostalCode);
            NormalizationUtils.AddToBuilderWithComma(ret, this.Country);

            return ret.ToString();
        }
    }
}
