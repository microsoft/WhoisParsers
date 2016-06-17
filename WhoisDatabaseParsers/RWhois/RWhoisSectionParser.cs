// -----------------------------------------------------------------------
// <copyright file="RWhoisSectionParser.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public class RWhoisSectionParser : ISectionParser
    {
        public RWhoisSectionParser()
        {
            this.ResetFieldStats();
        }

        public Dictionary<string, HashSet<string>> TypeToFieldNamesSet { get; set; }

        public Dictionary<string, List<string>> TypeToFieldNamesList { get; set; }

        public void ResetFieldStats()
        {
            this.TypeToFieldNamesSet = new Dictionary<string, HashSet<string>>();
            this.TypeToFieldNamesList = new Dictionary<string, List<string>>();
        }

        public RawWhoisSection Parse(string lines, string keyValueDelimitator = ":", string lineDelimintator = "\n")
        {
            if (lines == null)
            {
                throw new ArgumentException("lines should not be null");
            }

            return this.Parse(lines.Split(new string[] { lineDelimintator }, StringSplitOptions.RemoveEmptyEntries), keyValueDelimitator);
        }

        public RawWhoisSection Parse(IEnumerable<string> lines, string keyValueDelimitator = ":")
        {
            if (lines == null)
            {
                throw new ArgumentException("lines should not be null");
            }

            var records = new Dictionary<string, StringBuilder>();
            var validLineCounter = 0;
            string sectionType = null;
            string sectionId = null;
            HashSet<string> fieldNamesSet = null;
            List<string> fieldNamesList = null;

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#") && !line.StartsWith("%"))
                {
                    var parts = line.Split(new string[] { keyValueDelimitator }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 3)
                    {
                        validLineCounter++;
                        var key = parts[1];
                        string value;

                        if (parts.Length == 3)
                        {
                            value = parts[2].Trim();
                        }
                        else
                        {
                            value = string.Join(keyValueDelimitator, parts, 2, parts.Length - 1).Trim();
                        }

                        this.AddToRecord(records: records, fieldName: key, newValueLine: value);

                        if (validLineCounter == 1)
                        {
                            sectionType = parts[0];

                            if (!this.TypeToFieldNamesSet.TryGetValue(sectionType, out fieldNamesSet))
                            {
                                fieldNamesSet = new HashSet<string>();
                                this.TypeToFieldNamesSet.Add(sectionType, fieldNamesSet);
                            }

                            if (!this.TypeToFieldNamesList.TryGetValue(sectionType, out fieldNamesList))
                            {
                                fieldNamesList = new List<string>();
                                this.TypeToFieldNamesList.Add(sectionType, fieldNamesList);
                            }
                        }

                        if (!fieldNamesSet.Contains(key))
                        {
                            fieldNamesSet.Add(key);
                            fieldNamesList.Add(key);
                        }

                        if (key.ToLowerInvariant() == "id")
                        {
                            sectionId = value;
                        }
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "This line does not look like a triple: {0}", line));
                    }
                }
            }

            if (validLineCounter > 0 && !string.IsNullOrWhiteSpace(sectionType) && !string.IsNullOrWhiteSpace(sectionId) && records.Count > 0)
            {
                return new RawWhoisSection(sectionType, sectionId, records);
            }

            return null;
        }

        private void AddToRecord(Dictionary<string, StringBuilder> records, string fieldName, string newValueLine)
        {
            newValueLine = newValueLine.Trim();

            if (newValueLine.Length > 0)
            {
                StringBuilder currentValue;

                if (!records.TryGetValue(fieldName, out currentValue))
                {
                    currentValue = new StringBuilder();
                    records.Add(fieldName, currentValue);
                }

                if (currentValue.Length > 0)
                {
                    currentValue.AppendLine();
                }

                currentValue.Append(newValueLine);
            }
        }
    }
}
