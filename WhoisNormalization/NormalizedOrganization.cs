// -----------------------------------------------------------------------
// <copyright file="NormalizedOrganization.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    using System;
    using System.Collections.Generic;
    using Parsers;

    public class NormalizedOrganization : ICommonRecordMetadata
    {
        private static HashSet<string> organizationTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "org",
            "organization",
            "OrgID",
            "organisation"
        };

        private static HashSet<string> nameFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Org-Name",
            "Organization-Name",
            "Customer Organization",
            "Org-Name;I",
            "OrgName"
        };

        private static HashSet<string> phoneFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "phone"
        };

        public NormalizedOrganization()
        {
            this.Location = new NormalizedLocation();
        }

        ////// Common Records Metadata (ICommonRecordMetadata):

        public string Id { get; set; }

        public string Name { get; set; }

        public string Created { get; set; }

        public string Updated { get; set; }

        public string UpdatedBy { get; set; }

        public string Description { get; set; }

        public string Comment { get; set; }

        public string Source { get; set; }

        //////

        public NormalizedLocation Location { get; set; }

        public string Phone { get; set; }

        public static NormalizedOrganization TryParseFromSection(RawWhoisSection section)
        {
            if (organizationTypes.Contains(section.Type))
            {
                var organization = new NormalizedOrganization()
                {
                    Location = NormalizedLocation.TryParseFromSection(section),
                    Phone = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, phoneFields)
                };

                NormalizationUtils.ExtractCommonRecordMetadata(section, section.Id, nameFields, organization);

                return organization;
            }

            return null;
        }
    }
}
