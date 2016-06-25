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

        private bool connectForEachQuery = false;

        // Implements a subset of RFC 2167: http://projects.arin.net/rwhois/docs/rfc2167.txt
        public RawRWhoisClient(string hostname, int port) : base(hostname: hostname, port: port, autoConnect: false)
        {
            // Connect and read the version banner sent by the target RWhois server
            this.ConnectAndReadBanner();

            // The client sends a directive to hold the connection until it sends a directive to close the connection.
            // This setting allows querying the server multiple times using a single connection
            // See RFC 2167 Section 3.3.5
            this.WriteText(holdConnectCommand);

            // Get the response for the holdconnect command
            var holdConnectResponse = this.ReadText();

            if (string.IsNullOrWhiteSpace(holdConnectResponse))
            {
                // throw new ArgumentException("Did not receive a response when sending the holdconnect command");
                this.connectForEachQuery = true;
            }
            else
            {
                // Regarding the string[] delimitator array below: To avoid ambiguous results when strings in separator have 
                // characters in common, the Split operation proceeds from the beginning to the end of the value of the instance, 
                // and matches the first element in separator that is equal to a delimiter in the instance. The order in which 
                // substrings are encountered in the instance takes precedence over the order of elements in separator.
                // https://msdn.microsoft.com/en-us/library/tabh47cf(v=vs.110).aspx
                var holdConnectResponseLines = TextUtils.SplitTextToLines(text: holdConnectResponse, removeEmptyEntries: true);

                if (!RWhoisUtils.IsOkResponse(holdConnectResponseLines))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Did not receive a correct response after sending the holdconnect command. The response was: {0}", holdConnectResponse));
                }
            }
        }

        public string InitialServerBanner { get; set; }

        public string AnswerQuery(string query)
        {
            if (query == null)
            {
                throw new ArgumentException("query should not be null");
            }

            if (query.Contains("\r") || query.Contains("\n"))
            {
                throw new ArgumentException("query should not contain linefeed or carriage return (newline) characters");
            }

            if (this.connectForEachQuery)
            {
                this.ConnectAndReadBanner();
            }

            query = string.Format(CultureInfo.InvariantCulture, "{0}\r\n", query);
            this.WriteText(query);

            var responseTask = this.ReadTextAsync();
            responseTask.Wait();
            return responseTask.Result;
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

            if (this.connectForEachQuery)
            {
                this.ConnectAndReadBanner();
            }

            query = string.Format(CultureInfo.InvariantCulture, "{0}\r\n", query);
            this.WriteText(query);

            return await this.ReadTextAsync();
        }

        private void ConnectAndReadBanner()
        {
            this.Connect();

            // Read the initial banner sent by the server
            // The initial banner is mandatory as per RFC 2167 Section 3.1.9 
            this.InitialServerBanner = this.ReadText();
        }
    }
}
