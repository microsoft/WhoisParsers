// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Console
{
    using Microsoft.Geolocation.RWhois.Crawler;
    using System;
    using System.Globalization;
    using System.IO;
    using Whois.Normalization;
    using Whois.Parsers;
    using Whois.TsvExport;

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

            /*
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
            var organizationsToRefServers = ReferralServerFinder.FindOrganizationsToRefServers(settings: settings, organizationsFilePath: @"E:\Projects\Whois\ARIN\Raw\2019\01\17\arin_db.txt");

            Console.WriteLine("FindOrganizationsToRefRanges");
            var organizationsToRefRanges = ReferralServerFinder.FindOrganizationsToRefRanges(settings: settings, organizationsToRefServers: organizationsToRefServers, networksFilePath: @"E:\Projects\Whois\ARIN\Raw\2019\01\17\arin_db.txt");

            Console.WriteLine("RWhoisMultiCrawler");
            var multiCrawler = new RWhoisMultiCrawler(@"E:\Projects\Whois\ARIN-RWhois\Raw\2019\01\16\", attemptCrawlOrganizations: true);
            multiCrawler.CrawlInParallel(organizationsToRefServers, organizationsToRefRanges).Wait();
            */

            //// Diff:
            //// var organizationsWithRefServers = organizationsToRefServers.Keys;
            //// var organizationsWithRefRanges = organizationsToRefRanges.Keys;
            //// var organizationsWithoutRefRanges = organizationsWithRefServers.Except(organizationsWithRefRanges);

            ////var frtr = organizationsToRefRanges["FRTR"];

            /*
            var organizationsToRefServers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "FRTR",
                    "rwhois://rwhois.frontiernet.net:4321/"
                }
            };

            var organizationsToRefRanges = new Dictionary<string, HashSet<IPAddressRange>>(StringComparer.OrdinalIgnoreCase)
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
            var multiCrawler = new RWhoisMultiCrawler("./", attemptCrawlOrganizations: true);
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

            /*
            var parser = new WhoisParser(new RWhoisXferSectionTokenizer(), new RWhoisSectionParser());

            var completeInputPath = @"C:\Users\zmarty\lacnic.db";
            var completeOutputPath = @"C:\Users\zmarty\lacnic.tsv";

            var tsvWriter = new LacnicTsvWriter();
            tsvWriter.ExportIpv4RangesToTsv(completeInputPath, completeOutputPath);
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

            var organizationsToRefServers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "19EET",
                    "rwhois://rwhois.netelligent.ca:4321"
                }
            };

            var organizationsToRefRanges = new Dictionary<string, HashSet<IPAddressRange>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "19EET",
                    new HashSet<IPAddressRange>
                    {
                        IPAddressRange.Parse("199.193.52.0-199.193.55.255"),
                        IPAddressRange.Parse("208.82.120.0-208.82.123.255"),
                        IPAddressRange.Parse("204.147.76.0-204.147.79.255"),
                        IPAddressRange.Parse("209.44.104.144-209.44.104.159"),
                        IPAddressRange.Parse("209.44.97.0-209.44.97.15"),
                        IPAddressRange.Parse("64.15.74.0-64.15.74.15"),
                        IPAddressRange.Parse("64.15.78.0-64.15.78.15")
                    }
                }
            };

            Console.WriteLine("RWhoisMultiCrawler");
            var multiCrawler = new RWhoisMultiCrawler("./CrawlResults", attemptCrawlOrganizations: true);
            multiCrawler.CrawlInParallel(organizationsToRefServers, organizationsToRefRanges).Wait();
            */

            /*
            Console.WriteLine("RWhoisTSV");
            RWhoisTSV();

            Console.WriteLine("ArinTSV");
            ArinTSV();

            Console.WriteLine("AfrinicTSV");
            AfrinicTSV();
            */

            /*
            Console.WriteLine("ApnicTSV");
            ApnicTSV();
            */

            /*
            Console.WriteLine("LacnicTSV");
            LacnicTSV();
            */

            Console.WriteLine("Done!");
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
        }

        private static void RWhoisTSV()
        {
            var rwhoisTsvWriter = new RWhoisTsvWriter();
            rwhoisTsvWriter.NetworksWithLocationsToTsv(@"C:\Projects\Whois\ARIN-RWhois\Raw\2016\08\13\", @"C:\Projects\Whois\ARIN-RWhois\Processed\2016\08\13\2016-08-13-ARINRWhois-NetworkLocations.tsv");
            rwhoisTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\ARIN-RWhois\Raw\2016\08\13\", "Address", @"C:\Projects\Whois\ARIN-RWhois\Processed\2016\08\13\2016-08-13-ARINRWhois-TopAddresses.tsv");
            rwhoisTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\ARIN-RWhois\Raw\2016\08\13\", "Street", @"C:\Projects\Whois\ARIN-RWhois\Processed\2016\08\13\2016-08-13-ARINRWhois-TopStreets.tsv");
            rwhoisTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\ARIN-RWhois\Raw\2016\08\13\", "City", @"C:\Projects\Whois\ARIN-RWhois\Processed\2016\08\13\2016-08-13-ARINRWhois-TopCities.tsv");
            rwhoisTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\ARIN-RWhois\Raw\2016\08\13\", "Country", @"C:\Projects\Whois\ARIN-RWhois\Processed\2016\08\13\2016-08-13-ARINRWhois-TopCountries.tsv");
            rwhoisTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\ARIN-RWhois\Raw\2016\08\13\", "PostalCode", @"C:\Projects\Whois\ARIN-RWhois\Processed\2016\08\13\2016-08-13-ARINRWhois-TopPostalCodes.tsv");
        }

        private static void ArinTSV()
        {
            var arinTsvWriter = new ArinTsvWriter();
            arinTsvWriter.NetworksWithLocationsToTsv(@"C:\Projects\Whois\ARIN\Raw\2016\08\18\arin_db.txt", @"C:\Projects\Whois\ARIN\Processed\2016\08\18\2016-08-18-ARIN-NetworkLocations.tsv");
            arinTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\ARIN\Raw\2016\08\18\arin_db.txt", "Address", @"C:\Projects\Whois\ARIN\Processed\2016\08\18\2016-08-18-ARIN-TopAddresses.tsv");
            arinTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\ARIN\Raw\2016\08\18\arin_db.txt", "Street", @"C:\Projects\Whois\ARIN\Processed\2016\08\18\2016-08-18-ARIN-TopStreets.tsv");
            arinTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\ARIN\Raw\2016\08\18\arin_db.txt", "City", @"C:\Projects\Whois\ARIN\Processed\2016\08\18\2016-08-18-ARIN-TopCities.tsv");
            arinTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\ARIN\Raw\2016\08\18\arin_db.txt", "Country", @"C:\Projects\Whois\ARIN\Processed\2016\08\18\2016-08-18-ARIN-TopCountries.tsv");
            arinTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\ARIN\Raw\2016\08\18\arin_db.txt", "PostalCode", @"C:\Projects\Whois\ARIN\Processed\2016\08\18\2016-08-18-ARIN-TopPostalCodes.tsv");
        }

        private static void AfrinicTSV()
        {
            var afrinicTsvWriter = new AfrinicTsvWriter();
            afrinicTsvWriter.NetworksWithLocationsToTsv(@"C:\Projects\Whois\AFRINIC\Raw\2016\08\18\whois_dump", @"C:\Projects\Whois\AFRINIC\Processed\2016\08\18\2016-08-18-AFRINIC-NetworkLocations.tsv");
            afrinicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\AFRINIC\Raw\2016\08\18\whois_dump", "Address", @"C:\Projects\Whois\AFRINIC\Processed\2016\08\18\2016-08-18-AFRINIC-TopAddresses.tsv");
            afrinicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\AFRINIC\Raw\2016\08\18\whois_dump", "Street", @"C:\Projects\Whois\AFRINIC\Processed\2016\08\18\2016-08-18-AFRINIC-TopStreets.tsv");
            afrinicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\AFRINIC\Raw\2016\08\18\whois_dump", "City", @"C:\Projects\Whois\AFRINIC\Processed\2016\08\18\2016-08-18-AFRINIC-TopCities.tsv");
            afrinicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\AFRINIC\Raw\2016\08\18\whois_dump", "Country", @"C:\Projects\Whois\AFRINIC\Processed\2016\08\18\2016-08-18-AFRINIC-TopCountries.tsv");
            afrinicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\AFRINIC\Raw\2016\08\18\whois_dump", "PostalCode", @"C:\Projects\Whois\AFRINIC\Processed\2016\08\18\2016-08-18-AFRINIC-TopPostalCodes.tsv");
        }

        private static void ApnicTSV()
        {
            var apnicTsvWriter = new ApnicTsvWriter();
            apnicTsvWriter.NetworksWithLocationsToTsv(@"C:\Projects\Whois\APNIC\Raw\2016\08\18\apnic.RPSL.db", @"C:\Projects\Whois\APNIC\Processed\2016\08\18\2016-08-18-APNIC-NetworkLocations.tsv");
            apnicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\APNIC\Raw\2016\08\18\apnic.RPSL.db", "Address", @"C:\Projects\Whois\APNIC\Processed\2016\08\18\2016-08-18-APNIC-TopAddresses.tsv");
            apnicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\APNIC\Raw\2016\08\18\apnic.RPSL.db", "Street", @"C:\Projects\Whois\APNIC\Processed\2016\08\18\2016-08-18-APNIC-TopStreets.tsv");
            apnicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\APNIC\Raw\2016\08\18\apnic.RPSL.db", "City", @"C:\Projects\Whois\APNIC\Processed\2016\08\18\2016-08-18-APNIC-TopCities.tsv");
            apnicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\APNIC\Raw\2016\08\18\apnic.RPSL.db", "Country", @"C:\Projects\Whois\APNIC\Processed\2016\08\18\2016-08-18-APNIC-TopCountries.tsv");
            apnicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\APNIC\Raw\2016\08\18\apnic.RPSL.db", "PostalCode", @"C:\Projects\Whois\APNIC\Processed\2016\08\18\2016-08-18-APNIC-TopPostalCodes.tsv");
        }

        private static void LacnicTSV()
        {
            var lacnicTsvWriter = new LacnicTsvWriter();
            lacnicTsvWriter.NetworksWithLocationsToTsv(@"C:\Projects\Whois\LACNIC\Raw\2016\08\18\lacnic.db", @"C:\Projects\Whois\LACNIC\Processed\2016\08\18\2016-08-18-LACNIC-NetworkLocations.tsv");
            lacnicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\LACNIC\Raw\2016\08\18\lacnic.db", "Address", @"C:\Projects\Whois\LACNIC\Processed\2016\08\18\2016-08-18-LACNIC-TopAddresses.tsv");
            lacnicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\LACNIC\Raw\2016\08\18\lacnic.db", "Street", @"C:\Projects\Whois\LACNIC\Processed\2016\08\18\2016-08-18-LACNIC-TopStreets.tsv");
            lacnicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\LACNIC\Raw\2016\08\18\lacnic.db", "City", @"C:\Projects\Whois\LACNIC\Processed\2016\08\18\2016-08-18-LACNIC-TopCities.tsv");
            lacnicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\LACNIC\Raw\2016\08\18\lacnic.db", "Country", @"C:\Projects\Whois\LACNIC\Processed\2016\08\18\2016-08-18-LACNIC-TopCountries.tsv");
            lacnicTsvWriter.NetworksLocationPropertyCountsToTsv(@"C:\Projects\Whois\LACNIC\Raw\2016\08\18\lacnic.db", "PostalCode", @"C:\Projects\Whois\LACNIC\Processed\2016\08\18\2016-08-18-LACNIC-TopPostalCodes.tsv");
        }
    }
}
