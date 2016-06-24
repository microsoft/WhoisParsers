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
    using Utils;

    public class SectionParser : ISectionParser
    {
        private int skipParts = 0;

        public SectionParser(int skipParts = 0)
        {
            this.skipParts = skipParts;
            this.ResetFieldStats();
        }

        public Dictionary<string, HashSet<string>> TypeToFieldNamesSet { get; protected set; }

        public Dictionary<string, List<string>> TypeToFieldNamesList { get; protected set; }

        public void ResetFieldStats()
        {
            this.TypeToFieldNamesSet = new Dictionary<string, HashSet<string>>();
            this.TypeToFieldNamesList = new Dictionary<string, List<string>>();
        }

        public RawWhoisSection Parse(string lines, string keyValueDelimitator = ":")
        {
            if (lines == null)
            {
                throw new ArgumentException("lines should not be null");
            }

            return this.Parse(TextUtils.SplitTextToLines(text: lines, removeEmptyEntries: true), keyValueDelimitator);
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
            HashSet<string> localFieldNamesSet = new HashSet<string>();
            List<string> localFieldNamesList = new List<string>();

            string currentFieldName = null;
            string skipPartsOverrideType = null;

            foreach (var line in lines)
            {
                // Afrinic contains an invalid line like: DUMMY for 5490
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("dummy for ", StringComparison.OrdinalIgnoreCase) && !line.StartsWith("#") && !line.StartsWith("%"))
                {
                    var parts = line.Split(new string[] { keyValueDelimitator }, StringSplitOptions.RemoveEmptyEntries);

                    if (this.skipParts > 0 && parts.Length >= this.skipParts)
                    {
                        if (skipPartsOverrideType == null)
                        {
                            skipPartsOverrideType = parts[0];
                        }

                        var newParts = new string[parts.Length - this.skipParts];
                        Array.Copy(parts, this.skipParts, newParts, 0, newParts.Length);
                        parts = newParts;
                    }

                    var key = string.Empty;
                    var value = string.Empty;

                    if (key.Length < line.Length)
                    {
                        key = parts[0];
                    }

                    if (parts.Length > 1)
                    {
                        value = string.Join(keyValueDelimitator, parts, 1, parts.Length - 1).Trim();
                    }

                    if (key != string.Empty)
                    {
                        validLineCounter++;
                        currentFieldName = key;
                        this.AddToRecord(records: records, fieldName: currentFieldName, newValueLine: value);

                        if (validLineCounter == 1 && this.skipParts == 0)
                        {
                            sectionType = key;
                            sectionId = value;
                        }

                        if (string.Compare(key, "ID", ignoreCase: true) == 0)
                        {
                            sectionId = value;
                        }

                        if (!localFieldNamesSet.Contains(key))
                        {
                            localFieldNamesSet.Add(key);
                            localFieldNamesList.Add(key);
                        }
                    }
                    else if (currentFieldName != null)
                    {
                        this.AddToRecord(records: records, fieldName: currentFieldName, newValueLine: line);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "We tried to parse a partial record value when there was not current record, so we don't know where to append the partial record: {0}", line));
                    }
                }
            }

            if (skipPartsOverrideType != null)
            {
                sectionType = skipPartsOverrideType;
            }

            if (validLineCounter > 0 && !string.IsNullOrWhiteSpace(sectionType) && !string.IsNullOrWhiteSpace(sectionId) && records.Count > 0)
            {
                if (sectionType != null)
                {
                    HashSet<string> globalFieldNamesSet;
                    List<string> globalFieldNamesList;

                    if (!this.TypeToFieldNamesSet.TryGetValue(sectionType, out globalFieldNamesSet))
                    {
                        globalFieldNamesSet = new HashSet<string>();
                        this.TypeToFieldNamesSet.Add(sectionType, globalFieldNamesSet);
                    }

                    if (!this.TypeToFieldNamesList.TryGetValue(sectionType, out globalFieldNamesList))
                    {
                        globalFieldNamesList = new List<string>();
                        this.TypeToFieldNamesList.Add(sectionType, globalFieldNamesList);
                    }

                    foreach (var fieldName in localFieldNamesSet)
                    {
                        if (!globalFieldNamesSet.Contains(fieldName))
                        {
                            globalFieldNamesSet.Add(fieldName);
                            globalFieldNamesList.Add(fieldName);
                        }
                    }
                }

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
