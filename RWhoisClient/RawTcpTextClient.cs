// -----------------------------------------------------------------------
// <copyright file="RawTcpTextClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Client
{
    using System;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using NLog;

    public class RawTcpTextClient : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string hostname;
        private int port;

        private TcpClient client = null;
        private NetworkStream stream = null;

        private int receiveTimeout;
        private int sendTimeout;

        public RawTcpTextClient(string hostname, int port, int receiveTimeout = 5000, int sendTimeout = 5000)
        {
            this.hostname = hostname;
            this.port = port;
            this.receiveTimeout = receiveTimeout;
            this.sendTimeout = sendTimeout;
        }

        public async Task<string> ReadTextAsync(int readTimeoutMilli = 5000, int iterationDelayMilli = 200)
        {
            await this.EnsureClientIsConnected();
            return await this.stream.ReadTextAsync(readTimeoutMilli: readTimeoutMilli, iterationDelayMilli: iterationDelayMilli);
        }

        public async Task WriteTextAsync(string text)
        {
            await this.EnsureClientIsConnected();
            await this.stream.WriteText(text);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual async Task ConnectAsync()
        {
            this.client = new TcpClient()
            {
                    ReceiveTimeout = this.receiveTimeout,
                SendTimeout = this.sendTimeout
            };

            await this.client.ConnectAsync(this.hostname, this.port);
            this.stream = this.client.GetStream();
        }

        public void Disconnect()
        {
            try
            {
                this.client.GetStream().Close();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
            }

            try
            {
                this.client.Close();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.client != null)
                {
                    ((IDisposable)this.client).Dispose();
                    this.client = null;
                }
            }
        }

        private async Task EnsureClientIsConnected()
        {
            if (this.client == null)
            {
                throw new ArgumentNullException("client", "client is null, which probably means you did not call Connect() before using the client");
            }

            if (this.stream == null)
            {
                throw new ArgumentNullException("stream", "stream is null, which probably means you did not call Connect() before using the stream");
            }

            if (!this.client.Connected)
            {
                logger.Error("The client was not connected, so attempting to reconnect");
                await this.ConnectAsync();
            }
        }
    }
}
