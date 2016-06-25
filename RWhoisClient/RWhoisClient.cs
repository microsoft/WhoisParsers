// -----------------------------------------------------------------------
// <copyright file="RWhoisClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Client
{
    using System;
    using System.Collections.Generic;
    using Whois.Parsers;

    public class RWhoisClient : IDisposable
    {
        private RawRWhoisClient client;

        public RWhoisClient(string hostname, int port)
        {
            this.client = new RawRWhoisClient(hostname, port);
        }

        public IEnumerable<RawWhoisSection> RetrieveSectionsForQuery(IWhoisParser parser, string query)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            var result = this.client.AnswerQuery(query);

            if (result != null)
            {
                var sections = parser.RetrieveSectionsFromString(result);

                foreach (var section in sections)
                {
                    yield return section;
                }
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
                if (this.client != null)
                {
                    this.client.Dispose();
                    this.client = null;
                }
            }
        }
    }
}
