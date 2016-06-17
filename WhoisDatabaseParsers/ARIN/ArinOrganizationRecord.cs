// -----------------------------------------------------------------------
// <copyright file="ArinOrganizationRecord.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public class ArinOrganizationRecord
    {
        public ArinOrganizationRecord(RawWhoisSection rawSection)
        {
            if (rawSection == null)
            {
                throw new ArgumentException("rawSection should not be null");
            }

            if (rawSection.Type != "OrgID")
            {
                throw new ArgumentException("The record is not of type OrgID");
            }

            this.Type = rawSection.Type;
            this.Id = rawSection.Id;
            this.Street = null;
            this.City = null;
            this.StateProvince = null;
            this.PostalCode = null;
            this.Country = null;
            this.OrganizationName = null;
            this.AddressLooksValid = null;
            this.AddressWithoutOrganizationName = null;
            this.AddressWithOrganizationName = null;
            this.ReferralServer = null;

            this.ComputeAddress(rawSection.Records);

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

            StringBuilder comment;

            if (rawSection.Records.TryGetValue("Comment", out comment))
            {
                this.Comment = comment.ToString();
            }
            else
            {
                this.Comment = null;
            }

            StringBuilder referralServer;

            if (rawSection.Records.TryGetValue("ReferralServer", out referralServer))
            {
                this.ReferralServer = referralServer.ToString();
            }
            else
            {
                this.ReferralServer = null;
            }
        }

        public string Type { get; set; }

        public string Id { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string StateProvince { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string OrganizationName { get; set; }

        public bool? AddressLooksValid { get; set; }

        public string AddressWithoutOrganizationName { get; set; }

        public string AddressWithOrganizationName { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public DateTime? LastUpdatedDate { get; set; }

        public string Source { get; set; }

        public string Comment { get; set; }

        public string ReferralServer { get; set; }

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

            if (!string.IsNullOrWhiteSpace(this.Street))
            {
                ret.AppendFormat("Street: {0}\r\n", this.Street);
            }

            if (!string.IsNullOrWhiteSpace(this.City))
            {
                ret.AppendFormat("City: {0}\r\n", this.City);
            }

            if (!string.IsNullOrWhiteSpace(this.StateProvince))
            {
                ret.AppendFormat("StateProv: {0}\r\n", this.StateProvince);
            }

            if (!string.IsNullOrWhiteSpace(this.Country))
            {
                ret.AppendFormat("Country: {0}\r\n", this.Country);
            }

            if (!string.IsNullOrWhiteSpace(this.OrganizationName))
            {
                ret.AppendFormat("OrganizationName: {0}\r\n", this.OrganizationName);
            }
            
            if (this.AddressLooksValid != null)
            {
                ret.AppendFormat("AddressLooksValid: {0}\r\n", this.AddressLooksValid);
            }

            if (!string.IsNullOrWhiteSpace(this.AddressWithoutOrganizationName))
            {
                ret.AppendFormat("AddressWithoutOrganizationName: {0}\r\n", this.AddressWithoutOrganizationName);
            }

            if (!string.IsNullOrWhiteSpace(this.AddressWithoutOrganizationName))
            {
                ret.AppendFormat("AddressWithOrganizationName: {0}\r\n", this.AddressWithOrganizationName);
            }

            if (!string.IsNullOrWhiteSpace(this.ReferralServer))
            {
                ret.AppendFormat("ReferralServer: {0}\r\n", this.ReferralServer);
            }

            return ret.ToString();
        }

        private void ComputeAddress(Dictionary<string, StringBuilder> records)
        {
            var buffer = new StringBuilder();

            StringBuilder val;

            if (records.TryGetValue("Street", out val))
            {
                var street = val.ToString().Trim();

                if (street.Length > 0)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(", ");
                    }

                    this.Street = street;
                    buffer.Append(street);
                }
            }

            if (records.TryGetValue("City", out val))
            {
                var city = val.ToString().Trim();

                if (city.Length > 0)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(", ");
                    }

                    this.City = city;
                    buffer.Append(city);
                }
            }

            if (records.TryGetValue("State/Prov", out val))
            {
                var stateProv = val.ToString().Trim();

                if (stateProv.Length > 0)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(", ");
                    }

                    this.StateProvince = stateProv;
                    buffer.Append(stateProv);
                }
            }

            if (records.TryGetValue("PostalCode", out val))
            {
                var postalCode = val.ToString().Trim();

                if (postalCode.Length > 0)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(", ");
                    }

                    this.PostalCode = postalCode;
                    buffer.Append(postalCode);
                }
            }

            if (records.TryGetValue("Country", out val))
            {
                var country = val.ToString().Trim();

                if (country.Length > 0)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(", ");
                    }

                    this.Country = country;
                    buffer.Append(country);
                }
            }

            this.AddressWithoutOrganizationName = buffer.ToString();

            buffer = new StringBuilder();

            if (records.TryGetValue("OrgName", out val))
            {
                var organizationName = val.ToString().Trim();

                if (organizationName.Length > 0)
                {
                    this.OrganizationName = organizationName;
                    buffer.Append(organizationName);

                    if (!string.IsNullOrWhiteSpace(this.AddressWithoutOrganizationName))
                    {
                        buffer.Append(", ");
                        buffer.Append(this.AddressWithoutOrganizationName);
                        this.AddressWithOrganizationName = buffer.ToString();
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(this.Country) && !string.IsNullOrWhiteSpace(this.City))
            {
                this.AddressLooksValid = true;
            }
            else
            {
                this.AddressLooksValid = false;
            }
        }
    }
}
