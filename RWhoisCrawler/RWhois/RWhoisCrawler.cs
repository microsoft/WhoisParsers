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
    using NetTools;
    using RWhois.Client;
    using Whois.Parsers;
    using Whois.Utils;

    public class RWhoisCrawler : IObservable<RawWhoisSection>
    {
        private RWhoisClient client;
        private List<IObserver<RawWhoisSection>> observers;
        private HashSet<IPAddressRange> rangesPreviouslySeenByObservers;

        private int ipv4Increment;
        private BigInteger ipv6Increment;

        public RWhoisCrawler(string hostname, int port, IWhoisParser parser = null, int ipv4Increment = 16, BigInteger? ipv6Increment = null)
        {
            this.observers = new List<IObserver<RawWhoisSection>>();
            this.client = new RWhoisClient(hostname, port, parser);
            this.rangesPreviouslySeenByObservers = new HashSet<IPAddressRange>(new IPAddressRangeEqualityComparer());

            this.ipv4Increment = ipv4Increment;

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
        }

        public void CrawlRanges(IEnumerable<IPAddressRange> parentRanges)
        {
            if (parentRanges == null)
            {
                throw new ArgumentNullException("parentRanges");
            }

            foreach (var parentRange in parentRanges)
            {
                this.CrawlRange(parentRange);
            }
        }

        public void CrawlRange(IPAddressRange parentRange)
        {
            if (parentRange == null)
            {
                throw new ArgumentNullException("parentRange");
            }

            var successfullyCrawledInBulk = this.AttemptBulkCrawl(parentRange);

            if (!successfullyCrawledInBulk)
            {
                this.CrawlRangeGradually(parentRange);
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

        private bool AttemptBulkCrawl(IPAddressRange parentRange)
        {
            if (parentRange == null)
            {
                throw new ArgumentNullException("parentRange");
            }

            var receivedAnyResult = false;

            var query = string.Format(CultureInfo.InvariantCulture, "-xfer {0}", parentRange.ToCidrString());

            foreach (var section in this.client.RetrieveSectionsForQuery(query))
            {
                receivedAnyResult = true;

                var sectionIPRange = this.ExtractRangeFromSection(section);

                if (sectionIPRange != null)
                {
                    if (!this.rangesPreviouslySeenByObservers.Contains(sectionIPRange))
                    {
                        this.rangesPreviouslySeenByObservers.Add(sectionIPRange);

                        Console.WriteLine("Sending to observers range: " + sectionIPRange);

                        foreach (var observer in this.observers)
                        {
                            observer.OnNext(section);
                        }
                    }
                    else
                    {
                        ////Console.WriteLine("sectionIPRange: " + sectionIPRange + " already seen by observers");
                    }
                }
                else
                {
                    ////Console.WriteLine("Could not extract IP range from section: " + section);
                }
            }

            return receivedAnyResult;
        }

        private void CrawlRangeGradually(IPAddressRange parentRange)
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
                    Console.WriteLine("Did not query using this start IP before: " + currentStartIP);

                    previouslySeenStartIPs.Add(currentStartIP);

                    var query = currentStartIP.ToString();

                    var foundNewRange = false;

                    foreach (var section in this.client.RetrieveSectionsForQuery(query))
                    {
                        var sectionIPRange = this.ExtractRangeFromSection(section);

                        if (sectionIPRange != null)
                        {
                            if (!this.rangesPreviouslySeenByObservers.Contains(sectionIPRange))
                            {
                                foundNewRange = true;

                                this.rangesPreviouslySeenByObservers.Add(sectionIPRange);

                                Console.WriteLine("Sending to observers range: " + sectionIPRange);

                                foreach (var observer in this.observers)
                                {
                                    observer.OnNext(section);
                                }

                                this.TryEnqueueNewStartIP(previouslySeenStartIPs, startIPCrawlerQueue, parentRange, sectionIPRange);
                            }
                            else
                            {
                                ////Console.WriteLine("sectionIPRange: " + sectionIPRange + " already seen by observers");
                            }
                        }
                        else
                        {
                            ////Console.WriteLine("Could not extract IP range from section: " + section);
                        }
                    }

                    if (!foundNewRange)
                    {
                        this.FindNewStartIP(startIPCrawlerQueue, parentRange, currentStartIP);
                    }
                }
                else
                {
                    ////Console.WriteLine("Already seen this item: " + currentStartIP);
                }

                Thread.Sleep(1 * 1000);
                ////Console.WriteLine("Press a key to continue");
                ////Console.ReadKey();
            }
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
                ////Console.WriteLine("Adding incremented IP to startIPCrawlerQueue: " + newStartIP);
                startIPCrawlerQueue.Enqueue(newStartIP);
            }
        }

        private IPAddressRange ExtractRangeFromSection(RawWhoisSection section)
        {
            StringBuilder rawSectionRange;

            if (section.Records != null && section.Records.TryGetValue("IP-Network", out rawSectionRange))
            {
                IPAddressRange sectionIPRange;

                if (IPAddressRange.TryParse(rawSectionRange.ToString(), out sectionIPRange))
                {
                    return sectionIPRange;
                }
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
                    ////Console.WriteLine("-- Enqueuing new start IP: " + newIPStart);
                    startIPCrawlerQueue.Enqueue(newIPStart);
                }
                else
                {
                    ////Console.WriteLine("-- Already seen this start IP so we will not enqueue it: " + newIPStart);
                }
            }
            else
            {
                ////Console.WriteLine("-- newIPStart: " + newIPStart + " was larger than parentRange.End: " + parentRange.End);
            }
        }
    }
}
