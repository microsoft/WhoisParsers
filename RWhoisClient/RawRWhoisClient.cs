// -----------------------------------------------------------------------
// <copyright file="RawRWhoisClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NLog;
    using Whois.Utils;

    public class RawRWhoisClient : RawTcpTextClient
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string holdConnectCommand = "-holdconnect on\r\n";

        private static string xferCheckCommand = "-xfer\r\n";

        // Implements a subset of RFC 2167: http://projects.arin.net/rwhois/docs/rfc2167.txt
        public RawRWhoisClient(string hostname, int port, int receiveTimeout = 5000, int sendTimeout = 5000) : base(hostname, port, receiveTimeout, sendTimeout)
        {
            this.ConnectForEachQuery = false; // This is the default, we will set it to the right value in ConnectAsync
        }

        public bool ConnectForEachQuery { get; set; }

        public bool XferCommandSupported { get; set; }

        public string InitialServerBanner { get; set; }

        public override async Task ConnectAsync()
        {
            // Connect and read the version banner sent by the target RWhois server
            await this.ConnectAndReadBannerAsync();

            // The client sends a directive to hold the connection until it sends a directive to close the connection.
            // This setting allows querying the server multiple times using a single connection
            // See RFC 2167 Section 3.3.5
            await this.WriteTextAsync(holdConnectCommand);

            // Get the response for the holdconnect command
            var holdConnectResponse = await this.ReadTextAsync();

            var holdConnectResponseLines = TextUtils.SplitTextToLines(text: holdConnectResponse, removeEmptyEntries: true);

            if (!RWhoisUtils.IsOkResponse(holdConnectResponseLines))
            {
                this.ConnectForEachQuery = true;
            }

            this.XferCommandSupported = await this.CheckXferSupport();
        }

        public async Task<string> AnswerQueryAsync(string query)
        {
            if (query == null)
            {
                throw new ArgumentException("query should not be null");
            }

            if (query.Contains("\r") || query.Contains("\r"))
            {
                throw new ArgumentException("query should not contain linefeed or carriage return (newline) characters");
            }

            if (this.ConnectForEachQuery)
            {
                await this.ConnectAndReadBannerAsync();
            }

            query = string.Format(CultureInfo.InvariantCulture, "{0}\r\n", query);
            await this.WriteTextAsync(query);

            return await this.ReadTextAsync();
        }

        private async Task ConnectAndReadBannerAsync()
        {
            await base.ConnectAsync();

            // Read the initial banner sent by the server
            // The initial banner is mandatory as per RFC 2167 Section 3.1.9 
            this.InitialServerBanner = await this.ReadTextAsync();
        }

        // Checks if this server supports bulk commands (xfer)
        private async Task<bool> CheckXferSupport()
        {
            if (this.ConnectForEachQuery)
            {
                await this.ConnectAndReadBannerAsync();
            }

            await this.WriteTextAsync(xferCheckCommand);

            var xferCheckResponse = await this.ReadTextAsync();

            var parts = xferCheckResponse.Split(new char[] { ' ' });

            // Looking for: %error 338 Invalid directive syntax
            // This means -xfer is supported we just did not pass in the right parameters
            if (parts.Length >= 2 && string.Compare(parts[0], "%error", ignoreCase: true) == 0 && parts[1] == "338")
            {
                return true;
            }

            return false;
        }
    }
}
