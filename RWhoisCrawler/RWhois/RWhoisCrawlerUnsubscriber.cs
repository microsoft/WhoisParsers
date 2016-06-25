// -----------------------------------------------------------------------
// <copyright file="RWhoisCrawlerUnsubscriber.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Crawler
{
    using System;
    using System.Collections.Generic;
    using NLog;
    using Whois.Parsers;

    public class RWhoisCrawlerUnsubscriber : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private List<IObserver<RawWhoisSection>> observers;
        private IObserver<RawWhoisSection> currentObserver;

        public RWhoisCrawlerUnsubscriber(List<IObserver<RawWhoisSection>> observers, IObserver<RawWhoisSection> observer)
        {
            this.observers = observers;
            this.currentObserver = observer;
        }

        public void Dispose()
        {
            if (this.currentObserver != null && this.observers.Contains(this.currentObserver))
            {
                this.observers.Remove(this.currentObserver);
            }
        }
    }
}
