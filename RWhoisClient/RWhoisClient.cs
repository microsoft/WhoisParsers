// -----------------------------------------------------------------------
// <copyright file="RWhoisClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NLog;
    using Whois.Parsers;

    public class RWhoisClient : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public RWhoisClient(string hostname, int port, int receiveTimeout = 5000, int sendTimeout = 5000)
        {
            this.RawClient = new RawRWhoisClient(hostname, port, receiveTimeout, sendTimeout);
        }

        public RawRWhoisClient RawClient { get; set; }

        public async Task ConnectAsync()
        {
            await this.RawClient.ConnectAsync();
        }

        public async Task<IEnumerable<RawWhoisSection>> RetrieveSectionsForQueryAsync(IWhoisParser parser, string query)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            var result = await this.RawClient.AnswerQueryAsync(query);

            if (result != null)
            {
                return parser.RetrieveSectionsFromString(result);
            }

            return Enumerable.Empty<RawWhoisSection>();
        }

        public void Disconnect()
        {
            try
            {
                this.RawClient.Disconnect();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.RawClient != null)
                {
                    this.RawClient.Dispose();
                    this.RawClient = null;
                }
            }
        }
    }
}
