// -----------------------------------------------------------------------
// <copyright file="NetworkLocationExtraction.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    using System.Collections.Generic;
    using Parsers;
    using Utils;
    using System.Text;
    public class NetworkLocationExtraction
    {
        private static HashSet<string> OrganizationTypes = new HashSet<string>()
        {
            "org",
            "organization"
        };

        private static HashSet<string> OrganizationNameFields = new HashSet<string>()
        {
            "Org-Name"
        };

        private static HashSet<string> AddressFields = new HashSet<string>()
        {
            "Address"
        };

        private static HashSet<string> StreetFields = new HashSet<string>()
        {
            "Street",
            "Street-Address"
        };

        private static HashSet<string> CityFields = new HashSet<string>()
        {
            "City"
        };

        private static HashSet<string> StateFields = new HashSet<string>()
        {
            "State"
        };

        private static HashSet<string> PostalCodeFields = new HashSet<string>()
        {
            "Postal-Code"
        };

        private static HashSet<string> CountryFields = new HashSet<string>()
        {
            "Country",
            "Country-Code"
        };

        private static HashSet<string> PhoneFields = new HashSet<string>()
        {
            "Phone"
        };

        public IWhoisParser Parser { get; set; }

        private Dictionary<>

        public NetworkLocationExtraction(IWhoisParser parser)
        {
            this.Parser = parser;
        }

        public void ExtractLocations(string filePath)
        {
            foreach (var section in this.Parser.RetrieveSections(filePath))
            {
                if (OrganizationTypes.Contains(section.Type))
                {

                }
            }
        }

        public NormalizedOrganization ExtractNormalizedOrganization(RawWhoisSection section)
        {
            var organization = new NormalizedOrganization()
            {
                Id = section.Id,
                Name = this.FindRecordType(section, OrganizationNameFields),
                Address = this.FindRecordType(section, AddressFields),
                Street = this.FindRecordType(section, StreetFields),
                City = this.FindRecordType(section, CityFields),
                State = this.FindRecordType(section, StateFields),
                PostalCode = this.FindRecordType(section, PostalCodeFields),
                Country = this.FindRecordType(section, CountryFields),
                Phone = this.FindRecordType(section, PhoneFields)
            };

            if (organization.Id == null)
            {
                return null;
            }

            if (organization.Name == null)
            {
                return null;
            }

            if (!organization.AddressSeemsValid())
            {
                return null;
            }

            return organization;
        }

        private string FindRecordType(RawWhoisSection section, HashSet<string> fields)
        {
            foreach (var field in fields)
            {
                StringBuilder nameBuilder;

                if (section.Records.TryGetValue(field, out nameBuilder))
                {
                    return nameBuilder.ToString();
                }
            }

            return null;
        }
    }
}
