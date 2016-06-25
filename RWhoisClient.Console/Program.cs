// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Console
{
    using System;
    using Crawler;
    using Microsoft.Geolocation.Whois.Parsers;
    using NetTools;

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

            /*
            var settings = new ReferralServerFinderSettings()
            {
                Parser = new WhoisParser(new SectionTokenizer(), new SectionParser()),
                OrganizationIdField = "OrgID",
                NetworkIdField = "NetHandle",
                ReferralServerField = "ReferralServer",
                NetworkRangeField = "NetRange"
            };

            Console.WriteLine("FindOrganizationsToRefServers");
            var organizationsToRefServers = ReferralServerFinder.FindOrganizationsToRefServers(settings: settings, organizationsFilePath: @"C:\Users\ovidan\Downloads\arin\organizations.txt");

            Console.WriteLine("FindOrganizationsToRefRanges");
            var organizationsToRefRanges = ReferralServerFinder.FindOrganizationsToRefRanges(settings: settings, organizationsToRefServers: organizationsToRefServers, networksFilePath: @"C:\Users\ovidan\Downloads\arin\networks.txt");

            //// Diff:
            //// var organizationsWithRefServers = organizationsToRefServers.Keys;
            //// var organizationsWithRefRanges = organizationsToRefRanges.Keys;
            //// var organizationsWithoutRefRanges = organizationsWithRefServers.Except(organizationsWithRefRanges);

            Console.WriteLine("RWhoisMultiCrawler");
            var multiCrawler = new RWhoisMultiCrawler();
            multiCrawler.Crawl(organizationsToRefServers, organizationsToRefRanges);
            */

            Console.ReadKey();
        }
    }
}
