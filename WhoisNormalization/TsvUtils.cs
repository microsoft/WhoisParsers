// -----------------------------------------------------------------------
// <copyright file="TsvUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    using System.Collections.Generic;
    using System.Text;

    public static class TsvUtils
    {
        public static string GenerateTsvLine(Dictionary<string, StringBuilder> records, List<string> outputColumns)
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
                    ret.Append(ReplaceAndTrimIllegalCharacters(val.ToString()));
                }

                firstColumn = false;
            }

            return ret.ToString();
        }

        public static string ReplaceAndTrimIllegalCharacters(string text)
        {
            if (text == null)
            {
                return null;
            }

            return text.Replace("\t", " ").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Trim();
        }

        public static void AddToBuilderWithTab(StringBuilder builder, string text, bool firstColumn)
        {
            if (builder != null && text != null)
            {
                if (!firstColumn)
                {
                    builder.Append("\t");
                }

                builder.Append(ReplaceAndTrimIllegalCharacters(text));
            }
        }
    }
}
