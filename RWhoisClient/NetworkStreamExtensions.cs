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
    using System.Threading.Tasks;

    public static class NetworkStreamExtensions
    {
        public static async Task<string> ReadText(this NetworkStream stream, string[] delimitators = null, Encoding encoding = null)
        {
            if (delimitators == null)
            {
                delimitators = new string[] { "\r\n", "\r", "\n" };
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            var partialText = new StringBuilder();

            int returnedBytes;
            var buf = new byte[4096];

            while ((returnedBytes = await stream.ReadAsync(buf, 0, buf.Length)) >= 0)
            {
                if (returnedBytes == 0)
                {
                    // TODO: Double check
                    break;
                }

                var text = encoding.GetString(buf, 0, returnedBytes);
                partialText.Append(text);

                if (delimitators.Any(p => text.EndsWith(p)))
                {
                    return partialText.ToString();
                }
            }

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