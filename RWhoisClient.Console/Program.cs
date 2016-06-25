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
    using Crawler;

    public static class Program
    {
        public static void Main(string[] args)
        {
            /*
            var client = new RWhoisClient("rwhois.isomedia.com", 4321, new WhoisParser(new RWhoisXferSectionTokenizer(), new RWhoisSectionParser()));

            foreach (var section in client.RetrieveSectionsForQuery("-xfer 207.115.64.0/19"))
            {
                Console.WriteLine(section);
            }

            Console.ReadKey();
            */

            var crawler = new RWhoisCrawler("rwhois.frontiernet.net", 4321);
            var consumer = new RWhoisConsumer("frtr.txt");
            crawler.Subscribe(consumer);

            crawler.CrawlRange(IPAddressRange.Parse("104.169.0.0/16"));

            Console.ReadKey();
        }
    }
}
