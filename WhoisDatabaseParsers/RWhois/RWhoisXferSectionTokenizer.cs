// -----------------------------------------------------------------------
// <copyright file="RWhoisXferSectionTokenizer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System;
    using System.Collections.Generic;

    public class RWhoisXferSectionTokenizer : SectionTokenizer
    {
        /*
            Bulk records look like this: 

            %xfer network:Class-Name:network
            %xfer network:ID:NET-207-115-64-0-19
            %xfer network:Auth-Area:207.115.64.0/19
            %xfer network:Network-Name:NET-207-115-64-0-19
            %xfer network:IP-Network:207.115.64.0/19
            %xfer network:IP-Network-Block:207.115.64.0 - 207.115.95.255
            %xfer network:Organization:ISOMEDIA-INC
            %xfer network:Tech-Contact:support@isomedia.com
            %xfer network:Admin-Contact:hostmaster@isomedia.com
            %xfer network:Created:20050302
            %xfer network:Updated:20080303
            %xfer network:Updated-By:hostmaster@isomedia.com

            In this case we need to remove the %xfer and skip part the first colon separated record (network),
            so we configure the SectionTokenizer to ignore %xfer. In order to ignore the first colon separated 
            record (network) we need to pair this class with an instance of RWhoisSectionParser.
        */
        public RWhoisXferSectionTokenizer(string ignorePrefix = "%xfer") : base(ignorePrefix)
        {
        }
    }
}
