// -----------------------------------------------------------------------
// <copyright file="RWhoisUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Client
{
    using System;
    using System.Collections.Generic;

    public static class RWhoisUtils
    {
        public static bool IsOkResponse(List<string> responseLines)
        {
            if (responseLines != null && responseLines.Count > 0 && responseLines[responseLines.Count - 1].Trim() == "%ok")
            {
                return true;
            }

            return false;
        }

        // RFC 2167 Section 3.1.2: http://projects.arin.net/rwhois/docs/rfc2167.txt
        // A response is the information that a server returns to a client for a
        // directive.It is comprised of one or more lines, and the last line
        // always indicates the success or failure of the directive.The first
        // character of each response line must be a "%". If a server runs a
        // directive successfully, the last response line must be "%ok"
        // Otherwise, it must be "%error <error-code> <error-text>". A line with
        // the string "%ok" or "%error" in the first position must occur only  
        // once in a server response and must always be the last line. The
        // server may send the "%info" response for special messages.
        public static IEnumerable<string> CleanLines(string[] lines)
        {
            if (lines == null)
            {
                throw new ArgumentException("lines should not be null");
            }

            var keyValueDelimitator = ":";

            foreach (var line in lines)
            {
                if (!line.StartsWith("%", StringComparison.Ordinal))
                {
                    var parts = line.Split(new string[] { keyValueDelimitator }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 2)
                    {
                        var key = parts[0].Trim();
                        string value;

                        if (parts.Length == 2)
                        {
                            value = parts[1];
                        }
                        else
                        {
                            value = string.Join(keyValueDelimitator, parts, 1, parts.Length - 1);
                        }

                        if (value.Trim().Length > 0)
                        {
                            yield return value;
                        }
                    }
                }
            }
        }
    }
}
