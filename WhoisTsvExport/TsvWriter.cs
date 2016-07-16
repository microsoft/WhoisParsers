// -----------------------------------------------------------------------
// <copyright file="TsvWriter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Parsers;

    public class TsvWriter
    {
        public TsvWriter(IWhoisParser parser)
        {
            this.Parser = parser;
        }

        IWhoisParser Parser { get; set; }

        public void ExportFieldsToTsv(string inputFilePath, string outputFilePath, string recordType, List<string> outputColumns)
        {
            var organizations = this.Parser.RetrieveSections(inputFilePath, recordType);

            using (var outputFile = new StreamWriter(outputFilePath))
            {
                foreach (var rawOrganization in organizations)
                {
                    var tsvLine = TsvUtils.GenerateTsvLine(rawOrganization.Records, outputColumns);

                    outputFile.WriteLine(tsvLine);
                }
            }
        }

        public void ColumnsPerTypeToTsv(string inputFilePath, string outputFilePath)
        {
            var columnsPerTypes = this.Parser.ColumnsPerType(inputFilePath);

            using (var outputFile = new StreamWriter(outputFilePath))
            {
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
