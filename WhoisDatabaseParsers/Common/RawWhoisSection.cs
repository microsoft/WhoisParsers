// -----------------------------------------------------------------------
// <copyright file="RawWhoisSection.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System.Collections.Generic;
    using System.Text;
    using NetTools;

    public class RawWhoisSection
    {
        public RawWhoisSection(string type, string id, Dictionary<string, StringBuilder> records)
        {
            this.Type = type;
            this.Id = id;
            this.Records = records;
        }

        public string Type { get; set; }

        public string Id { get; set; }

        public Dictionary<string, StringBuilder> Records { get; set; }

        public override string ToString()
        {
            var ret = new StringBuilder();

            if (this.Records != null && this.Records.Count > 0)
            {
                foreach (var entry in this.Records)
                {
                    if (entry.Key != null && entry.Value != null)
                    {
                        ret.AppendFormat("{0}: {1}\r\n", entry.Key, entry.Value.ToString());
                    }
                }
            }

            return ret.ToString();
        }

        public string ToDebugString()
        {
            var ret = new StringBuilder();

            if (this.Records != null && this.Records.Count > 0)
            {
                foreach (var entry in this.Records)
                {
                    if (entry.Key == "IP-Network" && entry.Value != null)
                    {
                        IPAddressRange range;

                        if (IPAddressRange.TryParse(entry.Value.ToString(), out range))
                        {
                            ret.AppendFormat("{0}: {1} ({2} - {3})\r\n", entry.Key, entry.Value.ToString(), range.Begin, range.End);
                        }
                        else
                        {
                            ret.AppendFormat("{0}: {1} (could not parse range!)\r\n", entry.Key, entry.Value.ToString());
                        }
                    }
                    else
                    {
                        ret.AppendFormat("{0}: {1}\r\n", entry.Key, entry.Value.ToString());
                    }
                }
            }

            return ret.ToString();
        }
    }
}
