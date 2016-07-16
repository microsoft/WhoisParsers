// -----------------------------------------------------------------------
// <copyright file="TsvUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using System.Collections.Generic;
    using System.Text;

    public static class TsvUtils
    {
        public static string GenerateTsvLine(Dictionary<string, StringBuilder> records, List<string> outputColumns)
        {
            var ret = new StringBuilder();

            foreach (var outputColumn in outputColumns)
            {
                if (ret.Length > 0)
                {
                    ret.Append("\t");
                }

                StringBuilder val;

                if (records.TryGetValue(outputColumn, out val) && val != null)
                {
                    ret.Append(ReplaceAndTrimIllegalCharacters(val.ToString()));
                }
            }

            return ret.ToString();
        }

        public static string ReplaceAndTrimIllegalCharacters(string text)
        {
            return text.Replace("\t", " ").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Trim();
        }
    }
}
