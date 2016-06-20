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
        private IWhoisParser parser;

        public RWhoisClient(string hostname, int port, IWhoisParser parser)
        {
            this.client = new RawRWhoisClient(hostname, port);

            if (parser != null)
            {
                this.parser = parser;
            }
            else
            {
                this.parser = new WhoisParser(new SectionTokenizer(), new RWhoisSectionParser());
            }
        }

        public IEnumerable<RawWhoisSection> RetrieveSectionsForQuery(string query)
        {
            var result = this.client.AnswerQuery(query);

            if (result != null)
            {
                var sections = this.parser.RetrieveSectionsFromString(result);

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
