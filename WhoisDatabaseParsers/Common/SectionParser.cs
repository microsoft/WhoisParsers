// -----------------------------------------------------------------------
// <copyright file="SectionParser.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public class SectionParser : ISectionParser
    {
        public SectionParser()
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

            string currentFieldName = null;

            foreach (var line in lines)
            {
                // Afrinic contains an invalid line like: DUMMY for 5490
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("dummy for ", StringComparison.OrdinalIgnoreCase) && !line.StartsWith("#") && !line.StartsWith("%"))
                {
                    var keyIsValid = this.TupleKeyIsValid(line, keyValueDelimitator);

                    var parts = line.Split(new string[] { keyValueDelimitator }, StringSplitOptions.RemoveEmptyEntries);

                    if (line.StartsWith("\t", StringComparison.Ordinal) || line.StartsWith(" ", StringComparison.Ordinal) || line.StartsWith("+", StringComparison.Ordinal) || !keyIsValid || parts.Length == 1)
                    {
                        if (currentFieldName != null)
                        {
                            this.AddToRecord(records: records, fieldName: currentFieldName, newValueLine: line);
                        }
                        else
                        {
                            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "We tried to parse a partial record value when there was not current record, so we don't know where to append the partial record: {0}", line));
                        }
                    }
                    else if (parts.Length >= 2)
                    {
                        var key = parts[0].Trim();
                        string value;

                        if (parts.Length == 2)
                        {
                            value = parts[1].Trim();
                        }
                        else
                        {
                            value = string.Join(keyValueDelimitator, parts, 1, parts.Length - 1).Trim();
                        }

                        if (key.Length == 0)
                        {
                            throw new ArgumentException("The key of this tuple line is empty");
                        }

                        validLineCounter++;
                        currentFieldName = key;
                        this.AddToRecord(records: records, fieldName: currentFieldName, newValueLine: value);

                        if (validLineCounter == 1)
                        {
                            sectionType = key;
                            sectionId = value;

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
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "This line does not look like a tuple: {0}", line));
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

        private bool TupleKeyIsValid(string line, string keyValueDelimitator)
        {
            if (!string.IsNullOrEmpty(line) && line[0] != ' ' && line[0] != '\t' && line[0] != '+')
            {
                var parts = line.Split(new string[] { keyValueDelimitator }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 2)
                {
                    var key = parts[0].Trim();

                    if (key.Length > 0 && !key.Contains(" "))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
