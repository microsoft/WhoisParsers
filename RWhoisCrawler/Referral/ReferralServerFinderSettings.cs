// -----------------------------------------------------------------------
// <copyright file="ReferralServerFinderSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Crawler
{
    using Microsoft.Geolocation.Whois.Parsers;

    public class ReferralServerFinderSettings
    {
        public IWhoisParser Parser { get; set; }

        // OrgID
        public string OrganizationIdField { get; set; }

        // NetHandle
        public string NetworkIdField { get; set; }

        // ReferralServer
        public string ReferralServerField { get; set; }

        // NetRange
        public string NetworkRangeField { get; set; }

        public bool AnySettingsAreNull()
        {
            return
                this.Parser == null
                || this.OrganizationIdField == null
                || this.NetworkIdField == null
                || this.ReferralServerField == null
                || this.NetworkRangeField == null;
        }
    }
}
