// -----------------------------------------------------------------------
// <copyright file="IWhoisParser.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public interface IWhoisParser
    {
        Dictionary<string, List<string>> ColumnsPerTypeFromReader(StreamReader reader);

        Dictionary<string, List<string>> ColumnsPerTypeFromFile(string filePath);

        //// From reader

        IEnumerable<RawWhoisSection> RetrieveSectionsFromReader(StreamReader reader);

        IEnumerable<RawWhoisSection> RetrieveSectionsFromReader(StreamReader reader, string desiredType);

        IEnumerable<RawWhoisSection> RetrieveSectionsFromReader(StreamReader reader, HashSet<string> desiredTypes);

        //// From file

        IEnumerable<RawWhoisSection> RetrieveSectionsFromFile(string filePath);

        IEnumerable<RawWhoisSection> RetrieveSectionsFromFile(string filePath, string desiredType);

        IEnumerable<RawWhoisSection> RetrieveSectionsFromFile(string filePath, HashSet<string> desiredTypes);

        //// From block of text

        IEnumerable<RawWhoisSection> RetrieveSectionsFromString(string text);

        IEnumerable<RawWhoisSection> RetrieveSectionsFromString(string text, string desiredType);

        IEnumerable<RawWhoisSection> RetrieveSectionsFromString(string text, HashSet<string> desiredTypes);
    }
}
