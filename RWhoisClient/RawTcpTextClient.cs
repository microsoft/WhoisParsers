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

    public class RawTcpTextClient : IDisposable
    {
        private string hostname;
        private int port;

        private TcpClient client = null;
        private NetworkStream stream = null;

        public RawTcpTextClient(string hostname, int port, bool autoConnect = true)
        {
            this.hostname = hostname;
            this.port = port;

            if (autoConnect)
            {
                this.Connect();
            }
        }

        public async Task<string> ReadTextAsync(string[] delimitators = null)
        {
            this.EnsureClientIsConnected();
            return await this.stream.ReadText(delimitators);
        }

        public string ReadText(string[] delimitators = null)
        {
            this.EnsureClientIsConnected();

            /*
            // TODO
            if (this.stream.CanRead && this.stream.DataAvailable)
            {
            }
            */

            var readTextTask = this.stream.ReadText(delimitators);
            readTextTask.Wait();
            return readTextTask.Result;
        }

        public async void WriteTextAsync(string text)
        {
            this.EnsureClientIsConnected();
            await this.stream.WriteText(text);
        }

        public void WriteText(string text)
        {
            this.EnsureClientIsConnected();
            this.stream.WriteText(text).Wait();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Connect()
        {
            // TODO: Close current stream, if any
            this.client = new TcpClient(this.hostname, this.port);
            this.stream = this.client.GetStream();
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

        private void EnsureClientIsConnected()
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
                throw new ArgumentException("The client is not connected (anymore?)");
            }
        }
    }
}
