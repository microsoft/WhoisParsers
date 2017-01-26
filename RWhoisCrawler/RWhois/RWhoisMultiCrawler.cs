// -----------------------------------------------------------------------
// <copyright file="RWhoisMultiCrawler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using NetTools;
    using NLog;
    using System.Linq;

    public class RWhoisMultiCrawler
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static int defaultRefServerPort = 4321;
        private string outputPath;
        private bool attemptCrawlOrganizations;
        private List<string> failedConnectionServers = new List<string>();
        private List<string> unexpectedExceptions = new List<string>();

        public RWhoisMultiCrawler(string outputPath, bool attemptCrawlOrganizations = false)
        {
            this.outputPath = outputPath;

            if (!Directory.Exists(this.outputPath))
            {
                Directory.CreateDirectory(this.outputPath);
            }

            this.attemptCrawlOrganizations = attemptCrawlOrganizations;
        }

        public async Task CrawlOneByOne(Dictionary<string, string> organizationsToRefServers, Dictionary<string, HashSet<IPAddressRange>> organizationsToRefRanges)
        {
            foreach (var entry in organizationsToRefRanges)
            {
                var organizationId = entry.Key;
                var ranges = entry.Value;

                if (organizationId == null || ranges == null || ranges.Count == 0)
                {
                    continue;
                }

                string refServerUrl;
                Uri refServerUri;

                if (organizationsToRefServers.TryGetValue(organizationId, out refServerUrl) && Uri.TryCreate(refServerUrl, UriKind.Absolute, out refServerUri))
                {
                    var hostname = refServerUri.Host;
                    var port = refServerUri.Port;

                    var outFileName = string.Format(CultureInfo.InvariantCulture, "{0}.txt", organizationId);
                    var crawlerTask = this.CreateCrawlerTask(outFileName, hostname, port, ranges);
                    await crawlerTask;
                }
            }
        }

        public async Task CrawlInParallel(Dictionary<string, string> organizationsToRefServers, Dictionary<string, HashSet<IPAddressRange>> organizationsToRefRanges)
        {
            var crawlTasks = new List<Task>();

            foreach (var entry in organizationsToRefRanges)
            {
                var organizationId = entry.Key;
                var ranges = entry.Value;

                if (organizationId == null || ranges == null || ranges.Count == 0)
                {
                    continue;
                }

                string refServerUrl;
                Uri refServerUri;

                if (organizationsToRefServers.TryGetValue(organizationId, out refServerUrl) && Uri.TryCreate(refServerUrl, UriKind.Absolute, out refServerUri))
                {
                    var hostname = refServerUri.Host;
                    var port = refServerUri.Port;

                    var outFileName = string.Format(CultureInfo.InvariantCulture, "{0}.txt", organizationId);
                    var crawlerTask = this.CreateCrawlerTask(outFileName, hostname, port, ranges);
                    crawlTasks.Add(crawlerTask);
                }
            }

            logger.Debug("Starting all crawl tasks");
            await Task.WhenAll(crawlTasks.ToArray());
        }

        public async Task CrawlInParallelByServer(Dictionary<IPAddressRange, string> rangeToRefServer)
        {
            var crawlTasks = new List<Task>();
            var serverToRanges = new Dictionary<string, HashSet<IPAddressRange>>();
            serverToRanges = rangeToRefServer.GroupBy(r => r.Value).ToDictionary(x => x.Key, x => new HashSet<IPAddressRange>(x.Select(e => e.Key)));

            foreach (var serverAndRanges in serverToRanges)
            {
                var refServer = serverAndRanges.Key;
                var ranges = serverAndRanges.Value;
                var outFileName = string.Format(CultureInfo.InvariantCulture, "{0}.txt", refServer);
                var crawlerTask = this.CreateCrawlerTask(outFileName, refServer, defaultRefServerPort, ranges);
                crawlTasks.Add(crawlerTask);
            }

            failedConnectionServers.Clear();
            logger.Debug("Starting all crawl tasks");
            await Task.WhenAll(crawlTasks.ToArray());
            logger.Debug(string.Format("Unexpected exceptions: {0}", string.Join("\n", unexpectedExceptions)));
            logger.Debug(string.Format("The following servers have refused the connection: {0}", string.Join("; ", failedConnectionServers)));
            logger.Info(string.Format("Finished crawling RWhoIs servers. Total server count: {0}. Number of servers with failed connection: {1}", serverToRanges.Count(), failedConnectionServers.Count()));
        }

        private async Task CreateCrawlerTask(string outFileName, string hostname, int port, HashSet<IPAddressRange> ranges)
        {
            try
            {
                logger.Info(string.Format(CultureInfo.InvariantCulture, "Starting crawler for hostname: {0}, port: {1}", hostname, port));

                var crawler = new RWhoisCrawler(hostname, port, attemptCrawlOrganizations: this.attemptCrawlOrganizations);
                try
                {
                    await crawler.ConnectAsync();
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    failedConnectionServers.Add(hostname);
                    logger.Error(ex.Message);
                    logger.Error(string.Format("Connection failed for server: {0}", hostname));
                    return;
                }
                catch (System.IO.IOException ex)
                {
                    //The exception usually contains the following message: "Unable to read data from the transport connection: An established connection was aborted by the software in your host machine."
                    //That telnet request gives "error 503 Idle time exceeded" error
                    failedConnectionServers.Add(hostname);
                    logger.Error(ex.Message);
                    logger.Error(string.Format("Connection failed for server: {0}", hostname));
                    return;
                }

                var outFile = Path.Combine(this.outputPath, outFileName);

                var consumer = new RWhoisConsumer(outFile.ToString());

                crawler.Subscribe(consumer);
                await crawler.CrawlRangesAsync(ranges);

                logger.Info(string.Format(CultureInfo.InvariantCulture, "Done with crawler for hostname: {0}, port: {1}", hostname, port));
            }
            catch (Exception ex)
            {
                unexpectedExceptions.Add(string.Format("For {0}: {1}", hostname, ex.Message));
                logger.Error(ex);
            }
        }
    }
}
