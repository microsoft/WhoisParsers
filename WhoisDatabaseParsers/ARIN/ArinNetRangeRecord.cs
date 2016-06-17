// -----------------------------------------------------------------------
// <copyright file="ArinNetRangeRecord.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System;
    using System.Globalization;
    using System.Text;
    using NetTools;

    public class ArinNetRangeRecord
    {
        public ArinNetRangeRecord(RawWhoisSection rawSection)
        {
            if (rawSection == null)
            {
                throw new ArgumentException("rawSection should not be null");
            }

            if (rawSection.Type != "NetHandle" && rawSection.Type != "V6NetHandle")
            {
                throw new ArgumentException("The record is not of type NetHandle or V6NetHandle");
            }

            this.Type = rawSection.Type;
            this.Id = rawSection.Id;
            this.RangeIsValid = false;
            this.Range = null;
            this.RawRange = null;

            var rawNetRangeBuilder = new StringBuilder();
            IPAddressRange ipRange;

            if (rawSection.Records.TryGetValue("NetRange", out rawNetRangeBuilder))
            {
                var rawNetRange = rawNetRangeBuilder.ToString();
                this.RawRange = rawNetRange;

                rawNetRange = this.FixRawNetRange(rawNetRange);

                if (!string.IsNullOrWhiteSpace(rawNetRange) && IPAddressRange.TryParse(rawNetRange, out ipRange))
                {
                    this.RangeIsValid = true;
                    this.Range = ipRange;
                }
            }

            StringBuilder organizationId;

            if (rawSection.Records.TryGetValue("OrgID", out organizationId))
            {
                this.OrganizationId = organizationId.ToString();
            }
            else
            {
                this.OrganizationId = null;
            }

            StringBuilder parentId;

            if (rawSection.Records.TryGetValue("Parent", out parentId))
            {
                this.ParentId = parentId.ToString();
            }
            else
            {
                this.ParentId = null;
            }

            StringBuilder networkName;

            if (rawSection.Records.TryGetValue("NetName", out networkName))
            {
                this.NetworkName = networkName.ToString();
            }
            else
            {
                this.NetworkName = null;
            }

            StringBuilder networkType;

            if (rawSection.Records.TryGetValue("NetType", out networkType))
            {
                this.NetworkType = networkType.ToString();
            }
            else
            {
                this.NetworkType = null;
            }

            var englishUSCulture = new CultureInfo("en-US"); 

            StringBuilder registrationDateRaw;
            DateTime registrationDate;

            if (rawSection.Records.TryGetValue("RegDate", out registrationDateRaw) && DateTime.TryParseExact(s: registrationDateRaw.ToString(), format: "yyyy-MM-dd", provider: englishUSCulture, style: DateTimeStyles.None, result: out registrationDate))
            {
                this.RegistrationDate = registrationDate;
            }
            else
            {
                this.RegistrationDate = null;
            }

            StringBuilder lastUpdatedDateRaw;
            DateTime lastUpdatedDate;

            if (rawSection.Records.TryGetValue("Updated", out lastUpdatedDateRaw) && DateTime.TryParseExact(s: lastUpdatedDateRaw.ToString(), format: "yyyy-MM-dd", provider: englishUSCulture, style: DateTimeStyles.None, result: out lastUpdatedDate))
            {
                this.LastUpdatedDate = lastUpdatedDate;   
            }
            else
            {
                this.LastUpdatedDate = null;
            }

            StringBuilder source;

            if (rawSection.Records.TryGetValue("Source", out source))
            {
                this.Source = source.ToString();
            }
            else
            {
                this.Source = null;
            }

            StringBuilder originAS;

            if (rawSection.Records.TryGetValue("OriginAS", out originAS))
            {
                this.OriginAS = originAS.ToString();
            }
            else
            {
                this.OriginAS = null;
            }

            StringBuilder comment;

            if (rawSection.Records.TryGetValue("Comment", out comment))
            {
                this.Comment = comment.ToString();
            }
            else
            {
                this.Comment = null;
            }
        }

        public string Type { get; set; }

        public string Id { get; set; }

        public bool RangeIsValid { get; set; }

        public IPAddressRange Range { get; set; }

        public string RawRange { get; set; }

        public string OrganizationId { get; set; }

        public string ParentId { get; set; }

        public string NetworkName { get; set; }

        public string NetworkType { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public DateTime? LastUpdatedDate { get; set; }

        public string Source { get; set; }

        public string OriginAS { get; set; }

        public string Comment { get; set; }

        public override string ToString()
        {
            var ret = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(this.Type))
            {
                ret.AppendFormat("Type: {0}\r\n", this.Type);
            }

            if (!string.IsNullOrWhiteSpace(this.Id))
            {
                ret.AppendFormat("Id: {0}\r\n", this.Id);
            }

            if (this.RangeIsValid)
            {
                ret.AppendFormat("NetRange: {0} - {1} (family: {2})\r\n", this.Range.Begin, this.Range.End, this.Range.Begin.AddressFamily);
            }

            if (!string.IsNullOrWhiteSpace(this.RawRange))
            {
                ret.AppendFormat("RawNetRange: {0}", this.RawRange);
            }

            if (!string.IsNullOrWhiteSpace(this.OrganizationId))
            {
                ret.AppendFormat("OrgID: {0}\r\n", this.Id);
            }

            if (!string.IsNullOrWhiteSpace(this.ParentId))
            {
                ret.AppendFormat("Parent: {0}\r\n", this.ParentId);
            }

            if (!string.IsNullOrWhiteSpace(this.NetworkName))
            {
                ret.AppendFormat("NetName: {0}\r\n", this.NetworkName);
            }

            if (!string.IsNullOrWhiteSpace(this.NetworkType))
            {
                ret.AppendFormat("NetType: {0}\r\n", this.NetworkType);
            }

            if (this.RegistrationDate != null)
            {
                ret.AppendFormat("RegDate: {0}\r\n", this.RegistrationDate);
            }

            if (this.LastUpdatedDate != null)
            {
                ret.AppendFormat("Updated: {0}\r\n", this.LastUpdatedDate);
            }

            if (!string.IsNullOrWhiteSpace(this.Source))
            {
                ret.AppendFormat("Source: {0}\r\n", this.Source);
            }

            if (!string.IsNullOrWhiteSpace(this.OriginAS))
            {
                ret.AppendFormat("OriginAS: {0}\r\n", this.OriginAS);
            }

            if (!string.IsNullOrWhiteSpace(this.Comment))
            {
                ret.AppendFormat("Comment: {0}\r\n", this.Comment);
            }

            return ret.ToString();
        }

        private string FixRawNetRange(string rawNetRange)
        {
            if (string.IsNullOrWhiteSpace(rawNetRange))
            {
                return rawNetRange;
            }

            var parts = rawNetRange.Split(new char[] { '-' });

            if (parts.Length != 2)
            {
                return rawNetRange;
            }

            var leftPart = parts[0].Trim();
            leftPart = this.FixIPAddressColon(leftPart);

            var rightPart = parts[1].Trim();
            rightPart = this.FixIPAddressColon(rightPart);

            return string.Format(CultureInfo.InvariantCulture, "{0} - {1}", leftPart, rightPart);
        }

        private string FixIPAddressColon(string rawIPAddress)
        {
            if (string.IsNullOrWhiteSpace(rawIPAddress) || rawIPAddress.Length < 2)
            {
                return rawIPAddress;
            }

            if (rawIPAddress[rawIPAddress.Length - 1] == ':' && rawIPAddress[rawIPAddress.Length - 2] != ':')
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}:", rawIPAddress);
            }
            else
            {
                return rawIPAddress;
            }
        }
    }
}
