// -----------------------------------------------------------------------
// <copyright file="RWhoisTsvWriter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using Parsers;
    using System.Globalization;
    using System.IO;

    public class RWhoisTsvWriter
    {
        public RWhoisTsvWriter() : this(new WhoisParser(new SectionTokenizer(), new SectionParser()))
        {
        }

        public RWhoisTsvWriter(IWhoisParser parser)
        {
            this.Parser = parser;
        }

        public IWhoisParser Parser { get; set; }

        public void ColumnsPerTypeToTsv(string inputFolder, string outputFilePath)
        {
            using (var outputFile = new StreamWriter(outputFilePath))
            {
                foreach (var file in Directory.GetFiles(inputFolder))
                {
                    var columnsPerTypes = this.Parser.ColumnsPerType(file);

                    foreach (var columnsPerType in columnsPerTypes)
                    {
                        foreach (var column in columnsPerType.Value)
                        {
                            outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}\t{1}", columnsPerType.Key, column));
                        }
                    }
                }
            }
        }
    }
}
