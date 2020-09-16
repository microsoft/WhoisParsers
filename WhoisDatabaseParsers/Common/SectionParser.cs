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

        public Dictionary<string, int> TypeCounts { get; protected set; }

        public Dictionary<string, HashSet<string>> TypeToFieldNamesSet { get; protected set; }

        public Dictionary<string, List<string>> TypeToFieldNamesList { get; protected set; }

        public Dictionary<string, Dictionary<string, int>> TypeToFieldDistinctOcc { get; protected set; }

        public void ResetFieldStats()
        {
            this.TypeCounts = new Dictionary<string, int>();
            this.TypeToFieldNamesSet = new Dictionary<string, HashSet<string>>();
            this.TypeToFieldNamesList = new Dictionary<string, List<string>>();
            this.TypeToFieldDistinctOcc = new Dictionary<string, Dictionary<string, int>>();
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

            var records = new Dictionary<string, StringBuilder>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> localFieldNamesSet = new HashSet<string>();
            List<string> localFieldNamesList = new List<string>();

            string currentFieldName = null;

            string skipPartsOverrideType = null;

            // Will store the first key found in the record
            string firstKey = null;

            // Will store the first value of the first key found in the record
            // If the value of the first key actually spans multiple lines, 
            // this variable will only contain the first line of the value
            string firstKeyValue = null;

            foreach (var line in lines)
            {
                // Afrinic contains an invalid line like: DUMMY for 5490
                // TODO: Remove this if
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("dummy for ", StringComparison.OrdinalIgnoreCase) && !line.StartsWith("#") && !line.StartsWith("%"))
                {
                    // StringSplitOptions.None means leep empty records
                    var parts = line.Split(new string[] { keyValueDelimitator }, StringSplitOptions.None);

                    /*
                    If skipParts > 0 we need to remove one or more fields before we continue.
                    We do this for RWhois records. For these types of records we set skipParts to 1
                    to ignore the first field (network field below). Example:

                    network:Class-Name:network
                    network:ID:NET-207-115-64-0-19
                    network:Auth-Area:207.115.64.0/19
                    */
                    if (this.skipParts > 0 && parts.Length >= this.skipParts)
                    {
                        if (skipPartsOverrideType == null)
                        {
                            skipPartsOverrideType = parts[0];
                        }
                        else if (skipPartsOverrideType != parts[0])
                        {
                            // TODO: Log when the record type suddenly changes
                        }

                        var newParts = new string[parts.Length - this.skipParts];
                        Array.Copy(parts, this.skipParts, newParts, 0, newParts.Length);
                        parts = newParts;
                    }

                    if (parts.Length >= 2)
                    {
                        // If there are at least two parts it means we can parse both a key and a value
                        var key = parts[0].Trim();
                        var value = string.Join(keyValueDelimitator, parts, 1, parts.Length - 1).Trim();

                        if (key.Length > 0)
                        {
                            currentFieldName = key;
                            this.AddToRecord(records: records, fieldName: currentFieldName, newValueLine: value);

                            if (firstKey == null)
                            {
                                firstKey = key;
                                firstKeyValue = value;
                            }

                            if (!localFieldNamesSet.Contains(key))
                            {
                                localFieldNamesSet.Add(key);
                                localFieldNamesList.Add(key);
                            }
                        }
                        //// TODO: else log
                    }
                    else if (parts.Length == 1)
                    {
                        if (currentFieldName != null)
                        {
                            // If there is at least one part 
                            var value = parts[0];
                            this.AddToRecord(records: records, fieldName: currentFieldName, newValueLine: value);
                        }
                        //// TODO: else log
                    }
                    //// TODO: else log
                }
                //// TODO: else log
            }

            string sectionType = null;
            string sectionId = null;

            if (skipPartsOverrideType != null)
            {
                sectionType = skipPartsOverrideType;

                // If the records do not contain a Class-Name but this is a RWhois record
                // where the type is the first column, then create a Class-Name record
                if (!records.ContainsKey("Class-Name"))
                {
                    // TODO: Log
                    records["Class-Name"] = new StringBuilder(skipPartsOverrideType);
                }
            }

            // Try to locate the Class-Name and ID, wherever they are in the records
            var extractedClassName = RecordUtils.FindValueForKey(records, "Class-Name");
            var extractedId = RecordUtils.FindFirstValueForKeys(records, new List<string>() { "ID" });

            if (sectionType == null && extractedClassName != null)
            {
                sectionType = extractedClassName;
            }

            if (sectionId == null && extractedId != null)
            {
                sectionId = extractedId;
            }

            // We could not extract the type yet, so we will go looking for the value in the first 
            // record of the section
            if (sectionType == null && firstKey != null)
            {
                sectionType = firstKey;
            }
            //// TODO: else log

            // We could not extract the ID yet, so we will go looking for the key in the first 
            // record of the section
            if (sectionId == null && firstKeyValue != null)
            {
                sectionId = firstKeyValue;
            }
            //// TODO: else log

            if (!string.IsNullOrWhiteSpace(sectionType) && !string.IsNullOrWhiteSpace(sectionId) && records.Count > 0)
            {
                int globalSectionTypeCount;
                HashSet<string> globalFieldNamesSet;
                List<string> globalFieldNamesList;
                Dictionary<string, int> globalFieldOcc;

                if (!this.TypeCounts.TryGetValue(sectionType, out globalSectionTypeCount))
                {
                    globalSectionTypeCount = 0;
                }

                this.TypeCounts[sectionType] = globalSectionTypeCount + 1;

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

                if (!this.TypeToFieldDistinctOcc.TryGetValue(sectionType, out globalFieldOcc))
                {
                    globalFieldOcc = new Dictionary<string, int>();
                    TypeToFieldDistinctOcc[sectionType] = globalFieldOcc;
                }

                foreach (var fieldName in localFieldNamesSet)
                {
                    int occForField;

                    if (!globalFieldOcc.TryGetValue(fieldName, out occForField))
                    {
                        occForField = 0;
                    }

                    globalFieldOcc[fieldName] = occForField + 1;

                    if (!globalFieldNamesSet.Contains(fieldName))
                    {
                        globalFieldNamesSet.Add(fieldName);
                        globalFieldNamesList.Add(fieldName);
                    }
                }

                return new RawWhoisSection(sectionType, sectionId, records);
            }
            //// TODO: else log

            return null;
        }

        private void AddToRecord(Dictionary<string, StringBuilder> records, string fieldName, string newValueLine)
        {
            newValueLine = newValueLine.Trim();

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
