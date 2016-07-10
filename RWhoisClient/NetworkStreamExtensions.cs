// -----------------------------------------------------------------------
// <copyright file="NetworkStreamExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Client
{
    using System;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class NetworkStreamExtensions
    {
        public static async Task<string> ReadTextAsync(this NetworkStream stream, int readTimeoutMilli = 5000, int iterationDelayMilli = 200, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            var partialText = new StringBuilder();

            int returnedBytes;
            var buf = new byte[8192];

            do
            {
                await Task.Delay(iterationDelayMilli);

                returnedBytes = 0;

                if (partialText.Length == 0 || stream.DataAvailable)
                {
                    var readTask = stream.ReadAsync(buf, 0, buf.Length);
                    var timeoutTask = Task.Delay(readTimeoutMilli);

                    var couldReadBeforeTimeout = await Task.Factory.ContinueWhenAny<bool>(
                        new Task[] { readTask, timeoutTask },
                        (completedTask) =>
                        {
                            if (completedTask == timeoutTask)
                            {
                                stream.Close();
                                return false;
                            }
                            else
                            {
                                returnedBytes = readTask.Result;
                                return true;
                            }
                        });

                    if (couldReadBeforeTimeout)
                    {
                        if (returnedBytes > 0)
                        {
                            var text = encoding.GetString(buf, 0, returnedBytes);
                            partialText.Append(text);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Could not read text from stream");
                    }
                }
            }
            while (returnedBytes > 0 || (stream.CanRead && stream.DataAvailable));

            return partialText.ToString();
        }

        public static async Task WriteText(this NetworkStream stream, string text, Encoding encoding = null)
        {
            if (text == null)
            {
                throw new ArgumentException("text should not be null");
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            if (text.Length > 0)
            {
                var buf = encoding.GetBytes(text.ToCharArray());
                await stream.WriteAsync(buf, 0, buf.Length);
            }
        }
    }
}