// -----------------------------------------------------------------------
// <copyright file="NormalizedOrganization.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    public class NormalizedOrganization
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

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
    }
}
