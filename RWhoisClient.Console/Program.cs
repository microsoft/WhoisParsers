// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using NetTools;
    using Microsoft.Geolocation.RWhois.Client;
    using Microsoft.Geolocation.Whois.Parsers;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var client = new RWhoisClient("rwhois.isomedia.com", 4321, new WhoisParser(new RWhoisXferSectionTokenizer(), new RWhoisSectionParser()));

            foreach (var section in client.RetrieveSectionsForQuery("-xfer 207.115.64.0/19"))
            {
                Console.WriteLine(section);
            }

            Console.ReadKey();
        }
    }
}
