// -----------------------------------------------------------------------
// <copyright file="ISectionParser.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System;
    using System.Collections.Generic;

    public class RWhoisSectionParser : SectionParser
    {
        /*
            RWhois records look like this: 

            network:Class-Name:network
            network:ID:NET-207-115-64-0-19
            network:Auth-Area:207.115.64.0/19
            
            We need to skip part the first colon separated record (network), so we configure 
            the SectionParser to skip pass the network: part and use the rest of the line
        */
        public RWhoisSectionParser(int skipParts = 1) : base(skipParts)
        {
        }
    }
}
