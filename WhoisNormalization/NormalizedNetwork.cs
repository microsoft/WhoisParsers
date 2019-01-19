// -----------------------------------------------------------------------
// <copyright file="NormalizedOrganization.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NetTools;
    using Parsers;

    public class NormalizedNetwork : ICommonRecordMetadata
    {
        private static HashSet<string> networkTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "network",
            "Network",
            "NetHandle",
            "V6NetHandle",
            "inet6num",
            "inetnum"
        };

        private static HashSet<string> nameFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Network-Name",
            "Name",
            "Handle",
            "NetName"
        };

        private static HashSet<string> authAreaFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Auth-Area"
        };

        private static HashSet<string> ipRangeFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CIDR",
            "IP-Network",
            "IP-Network-Block",
            "Netblock",
            "IP-Range",
            "Network-Block",
            "NetRange",
            "inetnum",
            "inet6num"
        };

        private static HashSet<string> organizationFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Organization",
            "Organization;I",
            "OrgID",
            "org"
        };

        private static HashSet<string> originASFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "OriginAS"
        };

        private static HashSet<string> statusFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "status",
            "NetType"
        };

        public NormalizedNetwork()
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

        public string AuthArea { get; set; }

        public IPAddressRange IPRange { get; set; }

        public NormalizedLocation Location { get; set; }

        public NormalizedOrganization ExternalOrganization { get; set; }

        public string OriginAS { get; set; }

        public string Phone { get; set; }

        public string Status { get; set; }

        public static NormalizedNetwork TryParseFromSection(RawWhoisSection section)
        {
            if (networkTypes.Contains(section.Type))
            {
                var network = new NormalizedNetwork()
                {
                    Location = NormalizedLocation.TryParseFromSection(section),
                    AuthArea = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, authAreaFields),
                    OriginAS = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, originASFields),
                    Status = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, statusFields)
                };

                var candidateRanges = NormalizationUtils.FindAllMatchingFieldValuesInRecords(section, ipRangeFields);

                if (candidateRanges != null)
                {
                    IPAddressRange range = null;

                    foreach (var candidateRange in candidateRanges)
                    {
                        if (IPAddressRange.TryParse(candidateRange, out range))
                        {
                            break;
                        }
                    }

                    network.IPRange = range;
                }
                else
                {
                    // TODO: Some networks do not have an explicit IP range but maybe we can get it from the Auth Area or from the ID?
                }

                NormalizationUtils.ExtractCommonRecordMetadata(section, section.Id, nameFields, network);

                return network;
            }

            return null;
        }

        public void FindExternalOrganization(RawWhoisSection section, Dictionary<string, List<NormalizedOrganization>> organizations)
        {
            var candidateOrganizations = NormalizationUtils.FindAllMatchingFieldValuesInRecords(section, organizationFields);

            if (candidateOrganizations != null)
            {
                foreach (var candidateOrganization in candidateOrganizations)
                {
                    List<NormalizedOrganization> potentialOrganizations;

                    if (organizations.TryGetValue(candidateOrganization, out potentialOrganizations))
                    {
                        this.ExternalOrganization = potentialOrganizations.FirstOrDefault();
                        break;
                    }
                }
            }
        }

        public string ToLocationTsv()
        {
            var ret = new StringBuilder();

            TsvUtils.AddToBuilderWithTab(ret, this.Id, firstColumn: true);
            TsvUtils.AddToBuilderWithTab(ret, this.Name, firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.IPRange?.ToString(), firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.Location.ToString(), firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.AuthArea, firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.OriginAS, firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.Status, firstColumn: false);

            TsvUtils.AddToBuilderWithTab(ret, this.Created, firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.Updated, firstColumn: false);

            return ret.ToString();
        }

        public string ToDescriptionTsv()
        {
            var ret = new StringBuilder();

            TsvUtils.AddToBuilderWithTab(ret, this.Id, firstColumn: true);
            TsvUtils.AddToBuilderWithTab(ret, this.Name, firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.IPRange?.ToString(), firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.Description, firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.AuthArea, firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.OriginAS, firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.Status, firstColumn: false);

            TsvUtils.AddToBuilderWithTab(ret, this.Created, firstColumn: false);
            TsvUtils.AddToBuilderWithTab(ret, this.Updated, firstColumn: false);

            return ret.ToString();
        }
    }
}
