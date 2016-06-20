// -----------------------------------------------------------------------
// <copyright file="ISectionParser.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System.Collections.Generic;
    using System.Text;

    public interface ISectionParser
    {
        Dictionary<string, HashSet<string>> TypeToFieldNamesSet { get; set; }

        Dictionary<string, List<string>> TypeToFieldNamesList { get; set; }

        RawWhoisSection Parse(string lines, string keyValueDelimitator = ":");

        RawWhoisSection Parse(IEnumerable<string> lines, string keyValueDelimitator = ":");
    }
}
