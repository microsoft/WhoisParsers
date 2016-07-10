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
        Dictionary<string, List<string>> ColumnsPerType(StreamReader reader);

        Dictionary<string, List<string>> ColumnsPerType(string filePath);

        //// From reader

        IEnumerable<RawWhoisSection> RetrieveSections(StreamReader reader);

        IEnumerable<RawWhoisSection> RetrieveSections(StreamReader reader, string desiredType);

        IEnumerable<RawWhoisSection> RetrieveSections(StreamReader reader, HashSet<string> desiredTypes);

        //// From file

        IEnumerable<RawWhoisSection> RetrieveSections(string filePath);

        IEnumerable<RawWhoisSection> RetrieveSections(string filePath, string desiredType);

        IEnumerable<RawWhoisSection> RetrieveSections(string filePath, HashSet<string> desiredTypes);

        //// From block of text

        IEnumerable<RawWhoisSection> RetrieveSectionsFromString(string text);

        IEnumerable<RawWhoisSection> RetrieveSectionsFromString(string text, string desiredType);

        IEnumerable<RawWhoisSection> RetrieveSectionsFromString(string text, HashSet<string> desiredTypes);
    }
}
