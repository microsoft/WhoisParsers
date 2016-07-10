// -----------------------------------------------------------------------
// <copyright file="ReferralServerFinder.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using NetTools;

    public static class ReferralServerFinder
    {
        public static Dictionary<string, string> FindOrganizationsToRefServers(ReferralServerFinderSettings settings, string organizationsFilePath)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (settings.AnySettingsAreNull())
            {
                throw new ArgumentNullException("settings", "One or more settings are null");
            }

            if (organizationsFilePath == null)
            {
                throw new ArgumentNullException("organizationsFilePath");
            }

            if (!File.Exists(organizationsFilePath))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "File {0} does not exist", organizationsFilePath));
            }

            var organizationsToRefServers = new Dictionary<string, string>();

            foreach (var organization in settings.Parser.RetrieveSections(organizationsFilePath, settings.OrganizationIdField))
            {
                if (organization.Records != null)
                {
                    StringBuilder referralServer;

                    if (organization.Records.TryGetValue(settings.ReferralServerField, out referralServer))
                    {
                        organizationsToRefServers[organization.Id] = referralServer.ToString();
                    }
                }
            }

            return organizationsToRefServers;
        }

        public static Dictionary<string, HashSet<IPAddressRange>> FindOrganizationsToRefRanges(ReferralServerFinderSettings settings, Dictionary<string, string> organizationsToRefServers, string networksFilePath)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (organizationsToRefServers == null)
            {
                throw new ArgumentNullException("organizationsToRefServers");
            }

            if (settings.AnySettingsAreNull())
            {
                throw new ArgumentNullException("settings", "One or more settings are null");
            }

            if (networksFilePath == null)
            {
                throw new ArgumentNullException("networksFilePath");
            }

            if (!File.Exists(networksFilePath))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "File {0} does not exist", networksFilePath));
            }

            var organizationsToRefRanges = new Dictionary<string, HashSet<IPAddressRange>>();

            foreach (var network in settings.Parser.RetrieveSections(networksFilePath, settings.NetworkIdField))
            {
                if (network.Records != null)
                {
                    StringBuilder organizationIdRaw;

                    if (!network.Records.TryGetValue(settings.OrganizationIdField, out organizationIdRaw))
                    {
                        continue;
                    }

                    var organizationId = organizationIdRaw.ToString();

                    if (!organizationsToRefServers.ContainsKey(organizationId))
                    {
                        continue;
                    }

                    StringBuilder netRangeRaw;

                    if (!network.Records.TryGetValue(settings.NetworkRangeField, out netRangeRaw))
                    {
                        continue;
                    }

                    IPAddressRange netRange;

                    if (!IPAddressRange.TryParse(netRangeRaw.ToString(), out netRange))
                    {
                        continue;
                    }

                    HashSet<IPAddressRange> netRanges;

                    if (!organizationsToRefRanges.TryGetValue(organizationId, out netRanges))
                    {
                        netRanges = new HashSet<IPAddressRange>();
                        organizationsToRefRanges[organizationId] = netRanges;
                    }

                    netRanges.Add(netRange);
                }
            }

            return organizationsToRefRanges;
        }
    }
}
