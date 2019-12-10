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
    using System.Threading;
    using System.Threading.Tasks;
    using NetTools;
    using NLog;

    public class RWhoisMultiCrawler
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string outputPath;
        private bool attemptCrawlOrganizations;
        private CancellationTokenSource cancellationTokenSource;
        private TimeSpan crawlTimeLimit;

        public RWhoisMultiCrawler(string outputPath, bool attemptCrawlOrganizations = false, TimeSpan? crawlingTimeLimit = null)
        {
            this.outputPath = outputPath;

            if (!Directory.Exists(this.outputPath))
            {
                Directory.CreateDirectory(this.outputPath);
            }

            this.attemptCrawlOrganizations = attemptCrawlOrganizations;

            // supply a default of crawl time for one day
            this.crawlTimeLimit = crawlingTimeLimit ?? TimeSpan.FromDays(1);
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task CrawlOneByOne(Dictionary<string, string> organizationsToRefServers, Dictionary<string, HashSet<IPAddressRange>> organizationsToRefRanges)
        {
            // set the cancellation time limit before passing the cancellation token
            cancellationTokenSource.CancelAfter(this.crawlTimeLimit);

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

                    var crawlerTask = this.CreateCrawlerTask(organizationId, hostname, port, ranges, cancellationTokenSource.Token);
                    await crawlerTask;
                }
            }
        }

        public async Task CrawlInParallel(Dictionary<string, string> organizationsToRefServers, Dictionary<string, HashSet<IPAddressRange>> organizationsToRefRanges)
        {
            var crawlTasks = new List<Task>();

            // set the cancellation time limit before passing the cancellation token
            cancellationTokenSource.CancelAfter(this.crawlTimeLimit);

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

                    var crawlerTask = this.CreateCrawlerTask(organizationId, hostname, port, ranges, cancellationTokenSource.Token);
                    crawlTasks.Add(crawlerTask);
                }
            }

            logger.Debug("Starting all crawl tasks");
            await Task.WhenAll(crawlTasks.ToArray());
        }

        private async Task CreateCrawlerTask(string organizationId, string hostname, int port, HashSet<IPAddressRange> ranges, CancellationToken token)
        {
            logger.Info(string.Format(CultureInfo.InvariantCulture, "Starting crawler for organizationId: {0}, hostname: {1}, port: {2}", organizationId, hostname, port));

            var outFile = Path.Combine(this.outputPath, string.Format(CultureInfo.InvariantCulture, "{0}.txt", organizationId));
            var consumer = new RWhoisConsumer(outFile.ToString());

            try
            {
                var crawler = new RWhoisCrawler(hostname, port, attemptCrawlOrganizations: this.attemptCrawlOrganizations);
                await crawler.ConnectAsync();

                crawler.Subscribe(consumer);
                await crawler.CrawlRangesAsync(ranges, token);

                logger.Info(string.Format(CultureInfo.InvariantCulture, "Done with crawler for organizationId: {0}, hostname: {1}, port: {2}", organizationId, hostname, port));
                consumer.OnCompleted();
            }
            catch (OperationCanceledException ex)
            {
                logger.Warn(string.Format(CultureInfo.InvariantCulture, "Crawling was cancelled, exception raised: {0}", ex.Message));
                consumer.OnError(ex);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                consumer.OnError(ex);
            }
        }
    }
}
