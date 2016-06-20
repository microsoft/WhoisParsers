// -----------------------------------------------------------------------
// <copyright file="AfrinicSectionTokenizer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System;
    using System.IO;
    using System.Text;
    using Utils;

    public class AfrinicSectionTokenizer : ISectionTokenizer
    {
        public string RetrieveRecord(StreamReader reader)
        {
            if (reader == null || reader.EndOfStream)
            {
                return null;
            }

            string line = null;

            do
            {
                line = reader.ReadLine();

                if (line != null && line.Trim().Length > 0 && line.Trim() != "object")
                {
                    line = line.Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
                    return this.ReplaceComments(line);
                }
            }
            while (line != null);

            return null;
        }

        private string ReplaceComments(string line)
        {
            var ret = new StringBuilder();

            if (string.IsNullOrWhiteSpace(line))
            {
                return ret.ToString();
            }

            var parts = TextUtils.SplitTextToLines(text: line, removeEmptyEntries: true);

            foreach (var part in parts)
            {
                ret.AppendLine(part);
            }

            return ret.ToString();
        }
    }
}
