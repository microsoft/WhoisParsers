// -----------------------------------------------------------------------
// <copyright file="ISectionTokenizer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System.IO;
    using System.Text;

    public interface ISectionTokenizer
    {
        string RetrieveRecord(StreamReader reader);
    }
}
