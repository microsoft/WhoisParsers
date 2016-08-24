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
        static NormalizedLocation()
        {
            allBlacklistedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            allBlacklistedValues.UnionWith(blacklistedValuesSimilarToCountries);
            allBlacklistedValues.UnionWith(blacklistedValuesExceptSimilarToCountries);
        }

        private static HashSet<string> allBlacklistedValues;

        private static HashSet<string> blacklistedValuesExceptSimilarToCountries = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".",
            "0",
            "00000",
            "99999",
            "private",
            "private residence",
            "P.R.",
            "Private Address",
            "Private Addr",
            "Private Addr.",
            "Private Resident",
            "Private Res.",
            "PRIVATE CUSTOMER",
            "private-address",
            "Unavailable Street",
            "1 Unavailable Street",
            "Unavailable St",
            "Unavailable Str",
            "Unavailable St.",
            "Unavailable Str.",
            "Unavailable Address",
            "Unavailable",
            "Street Not Available",
            "Address Not Available",
            "Street N/A",
            "Address N/A",
            "N/A Street",
            "N/A Addr",
            "N/A Addr.",
            "N/A Address",
            "N/A St",
            "N/A St.",
            "N/A Str",
            "N/A Str.",
            "N/A",
            "n.a.",
            "n.a",
            "1 na",
            "Unknown",
            "Postal Address",
            "No info",
            "Not Defined",
            "Undefined",
            "null",
            "_None",
            "None",
            "Address",
            "Country",
            "City",
            "Postal Code",
            "Street",
            "PostalCode",
            "Private Data",
            "SERVER",
            "fake st",
            "fake st.",
            "FakePostalCode",
            "Fake",
            "FakeTown",
            "Fakeville",
            "FakeSuburb",
            "Fake_State",
            "Fake City",
            "Fakeplace"
        };

        private static HashSet<string> blacklistedValuesSimilarToCountries = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "NA",
            "NA NA",
            "n",
            "no_no"
        };

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
            "GeoLocation",
            "geoloc"
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
                Address = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, addressFields, allBlacklistedValues),
                Street = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, streetFields, allBlacklistedValues),
                City = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, cityFields, allBlacklistedValues),
                StateProvince = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, stateProvinceFields, allBlacklistedValues),
                PostalCode = RemoveExtraPrefix(NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, postalCodeFields, allBlacklistedValues)),
                Country = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, countryFields, blacklistedValuesExceptSimilarToCountries), // Not using blacklisted values because NA is a valid country
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

            // Note the Geolocation field is most likely longitude, latitude in this order
            if (!string.IsNullOrWhiteSpace(this.Geolocation))
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

        // Some postal codes look like this: Postal-Code: Code: W1J 6HL
        // We want to remove the extra Code: prefix
        private static string RemoveExtraPrefix(string text)
        {
            if (text != null)
            {
                var parts = text.Split(new char[] { ':' });

                if (parts.Length == 2)
                {
                    return parts[1].Trim();
                }
            }

            return text;
        }
    }
}
