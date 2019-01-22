// -----------------------------------------------------------------------
// <copyright file="WhoisParser.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;

    public class WhoisParser : IWhoisParser
    {
        public WhoisParser(ISectionTokenizer sectionTokenizer, ISectionParser sectionParser)
        {
            this.SectionTokenizer = sectionTokenizer;
            this.SectionParser = sectionParser;
        }

        public ISectionTokenizer SectionTokenizer { get; set; }

        public ISectionParser SectionParser { get; set; }

        public void ResetFieldStats()
        {
            this.SectionParser.ResetFieldStats();
        }

        public Dictionary<string, int> TypeCounts(StreamReader reader)
        {
            string record;

            while ((record = this.SectionTokenizer.RetrieveRecord(reader)) != null)
            {
                this.SectionParser.Parse(record);
            }

            return this.SectionParser.TypeCounts;
        }

        public Dictionary<string, int> TypeCounts(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                return this.TypeCounts(reader);
            }
        }

        public Dictionary<string, Dictionary<string, int>> TypeToFieldDistinctOcc(StreamReader reader)
        {
            string record;

            while ((record = this.SectionTokenizer.RetrieveRecord(reader)) != null)
            {
                this.SectionParser.Parse(record);
            }

            return this.SectionParser.TypeToFieldDistinctOcc;
        }

        public Dictionary<string, Dictionary<string, int>> TypeToFieldDistinctOcc(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                return this.TypeToFieldDistinctOcc(reader);
            }
        }

        public Dictionary<string, List<string>> ColumnsPerType(StreamReader reader)
        {
            string record;

            while ((record = this.SectionTokenizer.RetrieveRecord(reader)) != null)
            {
                this.SectionParser.Parse(record);
            }

            return this.SectionParser.TypeToFieldNamesList;
        }

        public Dictionary<string, List<string>> ColumnsPerType(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                return this.ColumnsPerType(reader);
            }
        }

        public IEnumerable<RawWhoisSection> RetrieveSections(StreamReader reader)
        {
            string record;

            while ((record = this.SectionTokenizer.RetrieveRecord(reader)) != null)
            {
                var section = this.SectionParser.Parse(record);

                if (section != null)
                {
                    yield return section;
                }
            }
        }

        public IEnumerable<RawWhoisSection> RetrieveSections(StreamReader reader, string desiredType)
        {
            return this.RetrieveSections(reader, new HashSet<string>() { desiredType });
        }

        public IEnumerable<RawWhoisSection> RetrieveSections(StreamReader reader, HashSet<string> desiredTypes)
        {
            string record;

            while ((record = this.SectionTokenizer.RetrieveRecord(reader)) != null)
            {
                if (record.Trim().Length > 0)
                {
                    var section = this.SectionParser.Parse(record);

                    if (section != null && desiredTypes.Contains(section.Type))
                    {
                        yield return section;
                    }
                }
            }
        }

        public IEnumerable<RawWhoisSection> RetrieveSections(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                foreach (var record in this.RetrieveSections(reader))
                {
                    yield return record;
                }
            }
        }

        public IEnumerable<RawWhoisSection> RetrieveSections(string filePath, string desiredType)
        {
            return this.RetrieveSections(filePath, new HashSet<string>() { desiredType });
        }

        public IEnumerable<RawWhoisSection> RetrieveSections(string filePath, HashSet<string> desiredTypes)
        {
            using (var reader = new StreamReader(filePath))
            {
                foreach (var section in this.RetrieveSections(reader, desiredTypes))
                {
                    yield return section;
                }
            }
        }

        public IEnumerable<RawWhoisSection> RetrieveSectionsFromString(string text)
        {
            return this.RetrieveSections(this.StreamFromString(text));
        }

        public IEnumerable<RawWhoisSection> RetrieveSectionsFromString(string text, string desiredType)
        {
            return this.RetrieveSections(this.StreamFromString(text), desiredType);
        }

        public IEnumerable<RawWhoisSection> RetrieveSectionsFromString(string text, HashSet<string> desiredTypes)
        {
            return this.RetrieveSections(this.StreamFromString(text), desiredTypes);
        }

        [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000", Justification = "This memory stream needs to be disposed of outside this function")]
        private StreamReader StreamFromString(string text)
        {
            MemoryStream memoryStream = null;

            try
            {
                var textBytes = Encoding.UTF8.GetBytes(text);
                memoryStream = new MemoryStream(textBytes);
                return new StreamReader(memoryStream);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
