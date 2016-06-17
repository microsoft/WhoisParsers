// -----------------------------------------------------------------------
// <copyright file="IPAddressRangeEqualityComparer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Utils
{
    using System;
    using System.Collections.Generic;
    using NetTools;

    public class IPAddressRangeEqualityComparer : IEqualityComparer<IPAddressRange>
    {
        public bool Equals(IPAddressRange x, IPAddressRange y)
        {
            return x.Begin.Equals(y.Begin) && x.End.Equals(y.End);
        }

        public int GetHashCode(IPAddressRange range)
        {
            var hash = 23;

            hash = unchecked((hash * 31) + range.Begin.GetHashCode());
            hash = unchecked((hash * 31) + range.End.GetHashCode());

            return hash;
        }
    }
}
