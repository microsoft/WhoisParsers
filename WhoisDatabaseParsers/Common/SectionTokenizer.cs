// -----------------------------------------------------------------------
// <copyright file="SectionTokenizer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public class SectionTokenizer : ISectionTokenizer
    {
        private string ignorePrefix;

        public SectionTokenizer(string ignorePrefix = null)
        {
            this.ignorePrefix = ignorePrefix;
        }

        public string RetrieveRecord(StreamReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentException("reader cannot be null");
            }

            if (reader.EndOfStream)
            {
                return null;
            }

            var ret = new StringBuilder();
            var stop = false;
            var linesCounter = 0;

            do
            {
                var line = reader.ReadLine();

                linesCounter++;

                if (linesCounter > 30000)
                {
                    var sample = ret.ToString();

                    if (ret.Length > 200)
                    {
                        sample = ret.ToString().Substring(0, 200);
                    }

                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "A section cannot contain more than 30,000 lines. Maybe the section delimitator is incorrect? First characters of buffer: {0}", sample));
                    //return ret.ToString();
                }

                if (line != null)
                {
                    if (this.ignorePrefix != null && line.StartsWith(this.ignorePrefix, ignoreCase: true, culture: CultureInfo.InvariantCulture))
                    {
                        line = line.Substring(this.ignorePrefix.Length).TrimStart();
                    }

                    if (line.Length == 0)
                    {
                        stop = true;
                    }
                    else
                    {
                        ret.AppendLine(line);
                    }
                }
                else
                {
                    stop = true;
                }
            }
            while (!stop);

            return ret.ToString();
        }
    }
}
