// -----------------------------------------------------------------------
// <copyright file="RWhoisTsvWriter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using Normalization;
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

        public void ColumnsPerTypeToTsv(string inputFolderPath, string outputFilePath)
        {
            var globalColumnsPerType = new Dictionary<string, List<string>>();

            foreach (var file in Directory.GetFiles(inputFolderPath))
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
                        outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}\t{1}", TsvUtils.ReplaceAndTrimIllegalCharacters(recordType, removeDoubleQuotes: true), TsvUtils.ReplaceAndTrimIllegalCharacters(recordColumn, removeDoubleQuotes: true)));
                    }
                }
            }
        }

        public void NetworksWithLocationsToSeparateTsv(string inputFolderPath, string outputFolderPath)
        {
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());

            foreach (var inputFilePath in Directory.GetFiles(inputFolderPath))
            {
                var locationExtraction = new NetworkLocationExtraction(parser);
                var outputFilePath = Path.Combine(outputFolderPath, Path.GetFileName(inputFilePath));

                using (var outputFile = new StreamWriter(outputFilePath))
                {
                    foreach (var network in locationExtraction.ExtractNetworksWithLocations(inputFilePath, inputFilePath))
                    {
                        if (network.Id != null && network.Location.AddressSeemsValid())
                        {
                            var networkTsv = network.ToLocationTsv();
                            outputFile.WriteLine(networkTsv);
                        }
                        //// TODO: else log
                    }
                }
            }
        }

        public void NetworksWithLocationsToTsv(string inputFolderPath, string outputFilePath)
        {
            var outputFolderPath = Path.GetDirectoryName(outputFilePath);

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());
            var locationExtraction = new NetworkLocationExtraction(parser);

            using (var outputFile = new StreamWriter(outputFilePath))
            {
                foreach (var inputFilePath in Directory.GetFiles(inputFolderPath))
                {
                    foreach (var network in locationExtraction.ExtractNetworksWithLocations(inputFilePath, inputFilePath))
                    {
                        if (network.Id != null)
                        {
                            var networkTsv = network.ToLocationTsv();
                            outputFile.WriteLine(networkTsv);
                        }
                        //// TODO: else log
                    }
                }
            }
        }

        public void NetworksLocationPropertyCountsToTsv(string inputFolderPath, string propertyName, string outputFilePath)
        {
            var outputFolderPath = Path.GetDirectoryName(outputFilePath);

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());
            var locationExtraction = new NetworkLocationExtraction(parser);

            var stringsCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var normalizedLocationType = typeof(NormalizedLocation);
            var properties = normalizedLocationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            PropertyInfo targetProperty = null;

            foreach (var property in properties)
            {
                if (property.Name == propertyName)
                {
                    targetProperty = property;
                }
            }

            if (targetProperty == null)
            {
                throw new ArgumentNullException("targetProperty");
            }

            foreach (var inputFilePath in Directory.GetFiles(inputFolderPath))
            {
                foreach (var network in locationExtraction.ExtractNetworksWithLocations(inputFilePath, inputFilePath))
                {
                    if (network.Id != null)
                    {
                        var rawPropertyValue = targetProperty.GetValue(network.Location);

                        if (rawPropertyValue != null)
                        {
                            var value = (string)rawPropertyValue;

                            int currentCount;

                            if (!stringsCount.TryGetValue(value, out currentCount))
                            {
                                currentCount = 0;
                            }

                            currentCount++;
                            stringsCount[value] = currentCount;
                        }
                    }
                    //// TODO: else log
                }
            }

            using (var outputFile = new StreamWriter(outputFilePath))
            {
                foreach (var entry in stringsCount)
                {
                    // No need to sanitize entry.Value since it's a number
                    outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}\t{1}", TsvUtils.ReplaceAndTrimIllegalCharacters(entry.Key, removeDoubleQuotes: true), entry.Value));
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
