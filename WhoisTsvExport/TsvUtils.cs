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
        public static string GenerateTsvLine(Dictionary<string, StringBuilder> records, List<string> outputColumns, bool removeDoubleQuotes = false)
        {
            var ret = new StringBuilder();
            var firstColumn = true;

            foreach (var outputColumn in outputColumns)
            {
                if (!firstColumn)
                {
                    ret.Append("\t");
                }

                StringBuilder val;

                if (records.TryGetValue(outputColumn, out val) && val != null)
                {
                    ret.Append(ReplaceAndTrimIllegalCharacters(val.ToString(), removeDoubleQuotes));
                }

                firstColumn = false;
            }

            return ret.ToString();
        }

        public static string ReplaceAndTrimIllegalCharacters(string text, bool removeDoubleQuotes = false)
        {
            if (text == null)
            {
                return null;
            }

            if (removeDoubleQuotes)
            {
                text = text.Replace("\"", string.Empty);
            }

            return text.Replace("\t", " ").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Trim();
        }

        public static void AddToBuilderWithTab(StringBuilder builder, string text, bool firstColumn)
        {
            if (builder != null)
            {
                if (!firstColumn)
                {
                    builder.Append("\t");
                }

                if (text != null)
                {
                    builder.Append(ReplaceAndTrimIllegalCharacters(text, removeDoubleQuotes: true));
                }
            }
        }
    }
}
