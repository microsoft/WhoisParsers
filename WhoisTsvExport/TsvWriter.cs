// -----------------------------------------------------------------------
// <copyright file="TsvWriter.cs" company="Microsoft">
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

    public class TsvWriter
    {
        public TsvWriter(IWhoisParser parser)
        {
            this.Parser = parser;
        }

        public IWhoisParser Parser { get; set; }

        public void ExportFieldsToTsv(string inputFilePath, string outputFilePath, string recordType, List<string> outputColumns)
        {
            var sections = this.Parser.RetrieveSections(inputFilePath, recordType);

            using (var outputFile = new StreamWriter(outputFilePath))
            {
                foreach (var section in sections)
                {
                    var tsvLine = TsvUtils.GenerateTsvLine(section.Records, outputColumns, removeDoubleQuotes: true);

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
                        outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}\t{1}", TsvUtils.ReplaceAndTrimIllegalCharacters(columnsPerType.Key, removeDoubleQuotes: true), TsvUtils.ReplaceAndTrimIllegalCharacters(column, removeDoubleQuotes: true)));
                    }
                }
            }
        }

        protected void NetworksWithLocationsToTsv(WhoisParser parser, string inputFilePath, string outputFilePath)
        {
            var outputFolderPath = Path.GetDirectoryName(outputFilePath);

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            var locationExtraction = new NetworkLocationExtraction(parser);

            using (var outputFile = new StreamWriter(outputFilePath))
            {
                foreach (var network in locationExtraction.ExtractNetworksWithLocations(inputFilePath, inputFilePath))
                {
                    if (network.Id != null && network.Location.AddressSeemsValid())
                    {
                        var networkTsv = network.ToLocationTsv();
                        outputFile.WriteLine(networkTsv);
                    }
                    // TODO: Else log
                }
            }
        }

        protected void NetworksLocationPropertyCountsToTsv(WhoisParser parser, string inputFilePath, string propertyName, string outputFilePath)
        {
            var outputFolderPath = Path.GetDirectoryName(outputFilePath);

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

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
                // TODO: Else log
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
    }
}
