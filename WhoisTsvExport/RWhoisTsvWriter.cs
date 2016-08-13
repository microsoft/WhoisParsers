// -----------------------------------------------------------------------
// <copyright file="RWhoisTsvWriter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Parsers;

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
            var globalColumnsPerType = new Dictionary<string, List<string>>();

            foreach (var file in Directory.GetFiles(inputFolder))
            {
                var localColumnsPerTypes = this.Parser.ColumnsPerType(file);
                this.MergeIntoGlobalColumnsPerType(globalColumnsPerType, localColumnsPerTypes);
            }

            using (var outputFile = new StreamWriter(outputFilePath))
            {
                foreach (var entry in globalColumnsPerType)
                {
                    var recordType = entry.Key;
                    var recordColumns = entry.Value;

                    foreach (var recordColumn in recordColumns)
                    {
                        outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}\t{1}", recordType, recordColumn));
                    }
                }
            }
        }

        private void MergeIntoGlobalColumnsPerType(Dictionary<string, List<string>> globalColumnsPerType, Dictionary<string, List<string>> localColumnsPerTypes)
        {
            foreach (var localEntry in localColumnsPerTypes)
            {
                var localRecordType = localEntry.Key;
                var localRecordColumns = localEntry.Value;

                List<string> globalRecordColumns;

                if (!globalColumnsPerType.TryGetValue(localRecordType, out globalRecordColumns))
                {
                    globalRecordColumns = new List<string>();
                }

                foreach (var localRecordColumn in localRecordColumns)
                {
                    if (!globalRecordColumns.Contains(localRecordColumn))
                    {
                        globalRecordColumns.Add(localRecordColumn);
                    }
                }

                globalColumnsPerType[localRecordType] = globalRecordColumns;
            }
        }
    }
}
