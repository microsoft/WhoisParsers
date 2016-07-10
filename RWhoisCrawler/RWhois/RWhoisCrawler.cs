// -----------------------------------------------------------------------
// <copyright file="RWhoisCrawler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Numerics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using NetTools;
    using NLog;
    using RWhois.Client;
    using Whois.Parsers;
    using Whois.Utils;

    public class RWhoisCrawler : IObservable<RawWhoisSection>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private RWhoisClient client;
        private IWhoisParser rwhoisParser;
        private IWhoisParser xferParser;

        private List<IObserver<RawWhoisSection>> observers;
        private HashSet<IPAddressRange> rangesPreviouslySeenByObservers;

        private int ipv4Increment;
        private BigInteger ipv6Increment;

        private int crawlIterationDelayMilli;

        public RWhoisCrawler(string hostname, int port, int? ipv4Increment = null, BigInteger? ipv6Increment = null, int crawlIterationDelayMilli = 100, IWhoisParser rwhoisParser = null, IWhoisParser xferParser = null)
        {
            this.client = new RWhoisClient(hostname, port);
            this.rwhoisParser = rwhoisParser;
            this.xferParser = xferParser;

            if (this.rwhoisParser == null)
            {
                this.rwhoisParser = new WhoisParser(new SectionTokenizer(), new RWhoisSectionParser());
            }

            if (this.xferParser == null)
            {
                this.xferParser = new WhoisParser(new RWhoisXferSectionTokenizer(), new RWhoisSectionParser());
            }

            this.observers = new List<IObserver<RawWhoisSection>>();
            this.rangesPreviouslySeenByObservers = new HashSet<IPAddressRange>(new IPAddressRangeEqualityComparer());

            if (ipv4Increment == null)
            {
                this.ipv4Increment = 16;
            }
            else
            {
                this.ipv4Increment = ipv4Increment.Value;
            }

            if (ipv6Increment == null)
            {
                // http://www.potato-people.com/blog/2009/02/ipv6-subnet-size-reference-table/
                // Business users can get a /48 IPv6 subnet, which is 2^80 IP addresses
                this.ipv6Increment = BigInteger.Pow(2, 80);
            }
            else
            {
                this.ipv6Increment = ipv6Increment.Value;
            }

            this.crawlIterationDelayMilli = crawlIterationDelayMilli;
        }

        public async Task ConnectAsync()
        {
            await this.client.ConnectAsync();
        }

        public async Task CrawlRangesAsync(IEnumerable<IPAddressRange> parentRanges)
        {
            if (parentRanges == null)
            {
                throw new ArgumentNullException("parentRanges");
            }

            foreach (var parentRange in parentRanges)
            {
                await this.CrawlRangeAsync(parentRange);
            }
        }

        public async Task CrawlRangeAsync(IPAddressRange parentRange)
        {
            if (parentRange == null)
            {
                throw new ArgumentNullException("parentRange");
            }

            if (this.client.RawClient.XferCommandSupported)
            {
                await this.CrawlRangeInBulk(parentRange);
            }
            else
            {
                await this.CrawlRangeGradually(parentRange);
            }
        }

        public IDisposable Subscribe(IObserver<RawWhoisSection> observer)
        {
            if (!this.observers.Contains(observer))
            {
                this.observers.Add(observer);
            }

            return new RWhoisCrawlerUnsubscriber(this.observers, observer);
        }

        private async Task CrawlRangeInBulk(IPAddressRange parentRange)
        {
            if (!await this.AttemptRawBulkCrawl(parentRange))
            {
                BigInteger defaultIncrementStep = 16;

                if (parentRange.Begin.AddressFamily == AddressFamily.InterNetwork)
                {
                    defaultIncrementStep = this.ipv4Increment;
                }
                else if (parentRange.Begin.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    defaultIncrementStep = this.ipv6Increment;
                }

                var incrementStep = defaultIncrementStep;

                var targetIP = parentRange.Begin;
                var alreadySeenAuthAreas = new HashSet<string>();

                while (targetIP.IsLessThanOrEqual(parentRange.End))
                {
                    var query = targetIP.ToString();

                    foreach (var section in await this.client.RetrieveSectionsForQueryAsync(this.rwhoisParser, query))
                    {
                        var authAreaName = this.ExtractValueFromSection(section, "Auth-Area");

                        if (authAreaName != null && !alreadySeenAuthAreas.Contains(authAreaName))
                        {
                            alreadySeenAuthAreas.Add(authAreaName);

                            var localMaxIP = await this.AttemptRawBulkCrawl(authAreaName.Trim());

                            if (targetIP.IsLessThan(localMaxIP))
                            {
                                logger.Info(string.Format(CultureInfo.InvariantCulture, "Update targetIP to: {0} while crawling auth area: {1}", localMaxIP, authAreaName));
                                targetIP = localMaxIP;

                                incrementStep = defaultIncrementStep;
                            }
                        }
                    }

                    var newTargetIP = targetIP.IncrementBy(incrementStep);
                    logger.Info(string.Format(CultureInfo.InvariantCulture, "Incremented targetIP from: {0} to: {1} (incrementStep: {2})", targetIP, newTargetIP, incrementStep));
                    targetIP = newTargetIP;

                    incrementStep = incrementStep + defaultIncrementStep;

                    await Task.Delay(this.crawlIterationDelayMilli);
                }
            }
        }

        private async Task CrawlRangeGradually(IPAddressRange parentRange)
        {
            if (parentRange == null)
            {
                throw new ArgumentNullException("parentRange");
            }

            var previouslySeenStartIPs = new HashSet<IPAddress>();
            var startIPCrawlerQueue = new Queue<IPAddress>();
            startIPCrawlerQueue.Enqueue(parentRange.Begin);

            while (startIPCrawlerQueue.Count > 0)
            {
                var currentStartIP = startIPCrawlerQueue.Dequeue();

                if (!previouslySeenStartIPs.Contains(currentStartIP))
                {
                    logger.Info(string.Format(CultureInfo.InvariantCulture, "First time querying using this start IP: {0}", currentStartIP));

                    previouslySeenStartIPs.Add(currentStartIP);

                    var query = currentStartIP.ToString();

                    var foundNewRange = false;

                    foreach (var section in await this.client.RetrieveSectionsForQueryAsync(this.rwhoisParser, query))
                    {
                        var sectionIPRange = this.ExtractRangeFromRecord(section, "IP-Network");

                        if (sectionIPRange != null)
                        {
                            if (!this.rangesPreviouslySeenByObservers.Contains(sectionIPRange))
                            {
                                foundNewRange = true;

                                this.rangesPreviouslySeenByObservers.Add(sectionIPRange);

                                logger.Info(string.Format(CultureInfo.InvariantCulture, "Sending to observers range: {0}", sectionIPRange));

                                foreach (var observer in this.observers)
                                {
                                    observer.OnNext(section);
                                }

                                this.TryEnqueueNewStartIP(previouslySeenStartIPs, startIPCrawlerQueue, parentRange, sectionIPRange);
                            }
                            else
                            {
                                logger.Debug(string.Format(CultureInfo.InvariantCulture, "Range already seen by observers: {0}", sectionIPRange));
                            }
                        }
                        else
                        {
                            logger.Debug(string.Format(CultureInfo.InvariantCulture, "Could not extract IP range from section: {0}", section));
                        }
                    }

                    if (!foundNewRange)
                    {
                        this.FindNewStartIP(startIPCrawlerQueue, parentRange, currentStartIP);
                    }
                }
                else
                {
                    logger.Debug(string.Format(CultureInfo.InvariantCulture, "Already seen this start IP: {0}", currentStartIP));
                }

                await Task.Delay(this.crawlIterationDelayMilli);
            }
        }

        private async Task<bool> AttemptRawBulkCrawl(IPAddressRange parentRange)
        {
            if (parentRange == null)
            {
                throw new ArgumentNullException("parentRange");
            }

            var maxIP = await this.AttemptRawBulkCrawl(parentRange.ToCidrString());

            return maxIP != null;
        }

        private async Task<IPAddress> AttemptRawBulkCrawl(string rangeName)
        {
            if (rangeName == null)
            {
                throw new ArgumentNullException("rangeName");
            }

            IPAddress maxIP = null;

            var query = string.Format(CultureInfo.InvariantCulture, "-xfer {0}", rangeName);

            foreach (var section in await this.client.RetrieveSectionsForQueryAsync(this.xferParser, query))
            {
                var sectionIPRange = this.ExtractRangeFromRecord(section, "IP-Network");

                if (sectionIPRange != null)
                {
                    if (maxIP == null)
                    {
                        maxIP = sectionIPRange.End;
                    }
                    else if (maxIP.IsLessThan(sectionIPRange.End))
                    {
                        maxIP = sectionIPRange.End;
                    }

                    if (!this.rangesPreviouslySeenByObservers.Contains(sectionIPRange))
                    {
                        this.rangesPreviouslySeenByObservers.Add(sectionIPRange);

                        logger.Info(string.Format(CultureInfo.InvariantCulture, "Sending range to observers: {0}", sectionIPRange));

                        foreach (var observer in this.observers)
                        {
                            observer.OnNext(section);
                        }
                    }
                    else
                    {
                        logger.Debug(string.Format(CultureInfo.InvariantCulture, "Range already seen by observers: {0}", sectionIPRange));
                    }
                }
                else
                {
                    logger.Debug(string.Format(CultureInfo.InvariantCulture, "Could not extract IP range from section: {0}", section));
                }
            }

            return maxIP;
        }

        private void FindNewStartIP(Queue<IPAddress> startIPCrawlerQueue, IPAddressRange parentRange, IPAddress currentStartIP)
        {
            if (parentRange == null)
            {
                throw new ArgumentNullException("parentRange");
            }

            if (parentRange.End == null)
            {
                throw new ArgumentNullException("parentRange.End");
            }

            if (currentStartIP == null)
            {
                throw new ArgumentNullException("currentStartIP");
            }

            IPAddress newStartIP;

            if (currentStartIP.AddressFamily == AddressFamily.InterNetwork)
            {
                newStartIP = currentStartIP.IncrementBy(this.ipv4Increment);
            }
            else if (currentStartIP.AddressFamily == AddressFamily.InterNetworkV6)
            {
                newStartIP = currentStartIP.IncrementBy(this.ipv6Increment);
            }
            else
            {
                throw new ArgumentException("The address family of currentStartIP is neither IPv4 nor IPv6", "currentStartIP");
            }

            if (newStartIP.IsLessThan(parentRange.End))
            {
                logger.Debug(string.Format(CultureInfo.InvariantCulture, "Adding incremented IP to startIPCrawlerQueue: {0}", newStartIP));
                startIPCrawlerQueue.Enqueue(newStartIP);
            }
        }

        private IPAddressRange ExtractRangeFromRecord(RawWhoisSection section, string fieldName)
        {
            StringBuilder rawSectionRange;

            if (section.Records != null && section.Records.TryGetValue(fieldName, out rawSectionRange))
            {
                IPAddressRange sectionIPRange;

                if (IPAddressRange.TryParse(rawSectionRange.ToString(), out sectionIPRange))
                {
                    return sectionIPRange;
                }
            }

            return null;
        }

        private string ExtractValueFromSection(RawWhoisSection section, string fieldName)
        {
            StringBuilder rawSectionRange;

            if (section.Records != null && section.Records.TryGetValue(fieldName, out rawSectionRange))
            {
                return rawSectionRange.ToString();
            }

            return null;
        }

        private void TryEnqueueNewStartIP(HashSet<IPAddress> previouslySeenStartIPs, Queue<IPAddress> startIPCrawlerQueue, IPAddressRange parentRange, IPAddressRange sectionIPRange)
        {
            var newIPStart = sectionIPRange.End.Increment();

            if (newIPStart.IsLessThanOrEqual(parentRange.End))
            {
                if (!previouslySeenStartIPs.Contains(newIPStart))
                {
                    logger.Debug(string.Format("Enqueuing new start IP: {0}", newIPStart));
                    startIPCrawlerQueue.Enqueue(newIPStart);
                }
                else
                {
                    logger.Debug(string.Format("Already seen this start IP so we will not enqueue it: {0}", newIPStart));
                }
            }
            else
            {
                logger.Debug(string.Format(CultureInfo.InvariantCulture, "newIPStart: {0} was larger than parentRange.End: {1}", newIPStart, parentRange.End));
            }
        }
    }
}
