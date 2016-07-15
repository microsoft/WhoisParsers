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

    public class RWhoisMultiCrawler
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string outputPath;

        public RWhoisMultiCrawler(string outputPath)
        {
            this.outputPath = outputPath;

            if (!Directory.Exists(this.outputPath))
            {
                Directory.CreateDirectory(this.outputPath);
            }
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

                    var crawlerTask = CreateCrawlerTask(organizationId, hostname, port, ranges);
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

                    var crawlerTask = CreateCrawlerTask(organizationId, hostname, port, ranges);
                    crawlTasks.Add(crawlerTask);
                }
            }

            logger.Debug("Starting all crawl tasks");
            await Task.WhenAll(crawlTasks.ToArray());
        }

        private async Task CreateCrawlerTask(string organizationId, string hostname, int port, HashSet<IPAddressRange> ranges)
        {
            try
            {
                logger.Info(string.Format(CultureInfo.InvariantCulture, "Starting crawler for organizationId: {0}, hostname: {1}, port: {2}", organizationId, hostname, port));

                var crawler = new RWhoisCrawler(hostname, port);
                await crawler.ConnectAsync();

                var outFile = Path.Combine(this.outputPath, string.Format(CultureInfo.InvariantCulture, "{0}.txt", organizationId));

                var consumer = new RWhoisConsumer(outFile.ToString());

                crawler.Subscribe(consumer);
                await crawler.CrawlRangesAsync(ranges);

                logger.Info(string.Format(CultureInfo.InvariantCulture, "Done with crawler for organizationId: {0}, hostname: {1}, port: {2}", organizationId, hostname, port));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
