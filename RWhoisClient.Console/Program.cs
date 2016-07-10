// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Console
{
    using System;
    using System.IO;
    using Crawler;
    using NetTools;

    public static class Program
    {
        public static void Main(string[] args)
        {
            /*
            var client = new RWhoisClient("rwhois.isomedia.com", 4321);
            client.ConnectAsync().Wait();

            var parser = new WhoisParser(new RWhoisXferSectionTokenizer(), new RWhoisSectionParser());

            var sectionsForQueryTask = client.RetrieveSectionsForQueryAsync(parser, "-xfer 207.115.64.0/19");
            sectionsForQueryTask.Wait();

            foreach (var section in sectionsForQueryTask.Result)
            {
                Console.WriteLine(section);
            }

            Console.WriteLine("Done!");
            Console.ReadKey();
            */

            /*
            var client = new RWhoisClient("rwhois.frontiernet.net", 4321);
            client.ConnectAsync().Wait();

            var parser = new WhoisParser(new RWhoisXferSectionTokenizer(), new RWhoisSectionParser());

            var sectionsForQueryTask = client.RetrieveSectionsForQueryAsync(parser, "184.8.0.0");
            sectionsForQueryTask.Wait();

            foreach (var section in sectionsForQueryTask.Result)
            {
                Console.WriteLine(section);
            }

            Console.WriteLine("Done!");
            */

            /* RWhois server is broken
            var client = new RWhoisClient("rwhois.hopone.net", 4321);
            client.ConnectAsync().Wait();

            Console.WriteLine(client.RawClient.XferCommandSupported);

            // 74.84.128.0

            Console.WriteLine("Done");
            */

            /*
            var client = new RWhoisClient("rwhois.frontiernet.net", 4321);
            client.ConnectAsync().Wait();

            Console.WriteLine(client.RawClient.XferCommandSupported);
            Console.WriteLine("Done");
            */

            var crawler = new RWhoisCrawler("rwhois.frontiernet.net", 4321, crawlIterationDelayMilli: 10000);
            crawler.ConnectAsync().Wait();

            var outFolder = "./CrawlResults";

            if (!Directory.Exists(outFolder))
            {
                Directory.CreateDirectory(outFolder);
            }

            var consumer = new RWhoisConsumer(Path.Combine(outFolder, "FRTR.txt"));
            crawler.Subscribe(consumer);

            crawler.CrawlRangeAsync(IPAddressRange.Parse("184.8.0.0/13")).Wait();

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
            var organizationsToRefServers = ReferralServerFinder.FindOrganizationsToRefServers(settings: settings, organizationsFilePath: @"C:\Users\zmarty\Downloads\arin\organizations.txt");

            Console.WriteLine("FindOrganizationsToRefRanges");
            var organizationsToRefRanges = ReferralServerFinder.FindOrganizationsToRefRanges(settings: settings, organizationsToRefServers: organizationsToRefServers, networksFilePath: @"C:\Users\zmarty\Downloads\arin\networks.txt");
            */

            //// Diff:
            //// var organizationsWithRefServers = organizationsToRefServers.Keys;
            //// var organizationsWithRefRanges = organizationsToRefRanges.Keys;
            //// var organizationsWithoutRefRanges = organizationsWithRefServers.Except(organizationsWithRefRanges);

            //var frtr = organizationsToRefRanges["FRTR"];

            /*
            var organizationsToRefServers = new Dictionary<string, string>()
            {
                {
                    "FRTR",
                    "rwhois://rwhois.frontiernet.net:4321/"
                }
            };

            var organizationsToRefRanges = new Dictionary<string, HashSet<IPAddressRange>>()
            {
                {
                    "FRTR",
                    new HashSet<IPAddressRange>()
                    {
                        IPAddressRange.Parse("184.8.0.0-184.15.255.255"),
                        IPAddressRange.Parse("216.37.128.0-216.37.255.255"),
                        IPAddressRange.Parse("199.224.64.0-199.224.127.255")
                    }
                }
            };
            */

            /*
            Console.WriteLine("RWhoisMultiCrawler");
            var multiCrawler = new RWhoisMultiCrawler("./CrawlResults");
            multiCrawler.Crawl(organizationsToRefServers, organizationsToRefRanges).Wait();
            */

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
            var organizationsToRefServers = ReferralServerFinder.FindOrganizationsToRefServers(settings: settings, organizationsFilePath: @"C:\Users\zmarty\Downloads\arin\organizations.txt");

            Console.WriteLine("FindOrganizationsToRefRanges");
            var organizationsToRefRanges = ReferralServerFinder.FindOrganizationsToRefRanges(settings: settings, organizationsToRefServers: organizationsToRefServers, networksFilePath: @"C:\Users\zmarty\Downloads\arin\networks.txt");

            //// Diff:
            //// var organizationsWithRefServers = organizationsToRefServers.Keys;
            //// var organizationsWithRefRanges = organizationsToRefRanges.Keys;
            //// var organizationsWithoutRefRanges = organizationsWithRefServers.Except(organizationsWithRefRanges);

            Console.WriteLine("RWhoisMultiCrawler");
            var multiCrawler = new RWhoisMultiCrawler("./");
            multiCrawler.Crawl(organizationsToRefServers, organizationsToRefRanges);
            */

            /*
            var settings = new ReferralServerFinderSettings()
            {
                Parser = new WhoisParser(new SectionTokenizer(), new SectionParser()),
                OrganizationIdField = "OrgID",
                NetworkIdField = "NetHandle",
                ReferralServerField = "ReferralServer",
                NetworkRangeField = "NetRange"
            };

            var organizationsToRefServers = ReferralServerFinder.FindOrganizationsToRefServers(settings: settings, organizationsFilePath: @"C:\Users\zmarty\Downloads\arin\organizations.txt");

            var liveServers = 0;

            var liveXferTrue = 0;
            var liveXferFalse = 0;

            var liveConnectForEachQueryTrue = 0;
            var liveConnectForEachQueryFalse = 0;

            var errors = 0;
            var total = 0;

            foreach (var item in organizationsToRefServers)
            {
                total++;

                try
                {
                    var serverUri = new Uri(item.Value);

                    var hostname = serverUri.Host;
                    var port = serverUri.Port;

                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", hostname, port));

                    var client = new RWhoisClient(hostname, port);
                    client.ConnectAsync().Wait();

                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "===> {0}:{1} -> {2}", hostname, port, client.RawClient.XferCommandSupported));

                    if (client.RawClient.ConnectForEachQuery)
                    {
                        liveConnectForEachQueryTrue++;
                    }
                    else
                    {
                        liveConnectForEachQueryFalse++;
                    }

                    if (client.RawClient.XferCommandSupported)
                    {
                        liveXferTrue++;
                    }
                    else
                    {
                        liveXferFalse++;
                    }

                    liveServers++;

                    client.Disconnect();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex);
                    Console.WriteLine("===> Error");
                    errors++;
                }

                Console.WriteLine();

                if (total % 10 == 0)
                {
                    Console.WriteLine("Results so far:");
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "liveServers = {0}", liveServers));
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "liveXferTrue = {0}", liveXferTrue));
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "liveXferFalse = {0}", liveXferFalse));
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "liveConnectForEachQueryTrue = {0}", liveConnectForEachQueryTrue));
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "liveConnectForEachQueryFalse = {0}", liveConnectForEachQueryFalse));
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "errors = {0}", errors));
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "total = {0}", total));
                    Console.WriteLine();
                }
            }

            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "liveServers = {0}", liveServers));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "liveXferTrue = {0}", liveXferTrue));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "liveXferFalse = {0}", liveXferFalse));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "liveConnectForEachQueryTrue = {0}", liveConnectForEachQueryTrue));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "liveConnectForEachQueryFalse = {0}", liveConnectForEachQueryFalse));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "errors = {0}", errors));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "total = {0}", total));
            */

            Console.WriteLine("Done!");
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
        }
    }
}
