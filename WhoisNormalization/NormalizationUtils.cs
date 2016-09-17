// -----------------------------------------------------------------------
// <copyright file="NormalizationUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using Parsers;
    using System.Globalization;

    public static class NormalizationUtils
    {
        private static HashSet<string> updatedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Updated",
            "changed"
        };

        private static HashSet<string> updatedByFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Updated-By"
        };

        private static HashSet<string> createdFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Created",
            "RegDate"
        };

        private static HashSet<string> descriptionFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Description",
            "descr"
        };

        private static HashSet<string> commentFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Comment",
            "Note",
            "remarks"
        };

        private static HashSet<string> sourceFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "source"
        };

        public static string FindFirstMatchingFieldValueInRecords(RawWhoisSection section, HashSet<string> fields, HashSet<string> blacklistedValues = null)
        {
            if (section != null && fields != null)
            {
                foreach (var field in fields)
                {
                    StringBuilder nameBuilder;

                    if (section.Records.TryGetValue(field, out nameBuilder))
                    {
                        var value = nameBuilder.ToString();

                        if (blacklistedValues == null || !blacklistedValues.Contains(value))
                        {
                            return value;
                        }
                    }
                }
            }

            return null;
        }

        public static List<string> FindAllMatchingFieldValuesInRecords(RawWhoisSection section, HashSet<string> fields)
        {
            var results = new List<string>();

            if (section != null && fields != null)
            {
                foreach (var field in fields)
                {
                    StringBuilder nameBuilder;

                    if (section.Records.TryGetValue(field, out nameBuilder))
                    {
                        results.Add(nameBuilder.ToString());
                    }
                }
            }

            if (results.Count > 0)
            {
                return results;
            }
            else
            {
                return null;
            }
        }

        public static void ExtractCommonRecordMetadata(RawWhoisSection section, string id, HashSet<string> nameFieldNames, ICommonRecordMetadata target)
        {
            target.Id = id;
            target.Name = FindFirstMatchingFieldValueInRecords(section, nameFieldNames);
            target.Created = FindOldestDate(FindFirstMatchingFieldValueInRecords(section, createdFields));
            target.Updated = FindOldestDate(FindFirstMatchingFieldValueInRecords(section, updatedFields));
            target.UpdatedBy = FindFirstMatchingFieldValueInRecords(section, updatedByFields);
            target.Description = FindFirstMatchingFieldValueInRecords(section, descriptionFields);
            target.Comment = FindFirstMatchingFieldValueInRecords(section, commentFields);
            target.Source = FindFirstMatchingFieldValueInRecords(section, sourceFields);
        }

        public static void AddToBuilderWithComma(StringBuilder builder, string text)
        {
            if (builder != null && text != null)
            {
                var trimmedText = text.Trim();

                if (trimmedText.Length > 0)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }

                    builder.Append(trimmedText);
                }
            }
        }

        public static string FindOldestDate(string text)
        {
            if (text == null)
            {
                return null;
            }

            var words = new List<string>(text.Split(new char[] { ' ' }));
            words = words.Select(word => word.Trim()).ToList<string>();
            words = words.Where(word => word.Length > 0).ToList<string>();

            DateTime? oldestParsedDate = null;

            string[] dateFormats = { "yyyy-dd-MM", "yyyyddMM" };

            foreach (var word in words)
            {
                DateTime currentParsedDate;

                if (DateTime.TryParseExact(s: word, formats: dateFormats, provider: new CultureInfo("en-US"), style: DateTimeStyles.None, result: out currentParsedDate))
                {
                    if (oldestParsedDate == null)
                    {
                        oldestParsedDate = currentParsedDate;
                    }
                    else if (oldestParsedDate < currentParsedDate)
                    {
                        oldestParsedDate = currentParsedDate;
                    }
                }
            }

            if (oldestParsedDate.HasValue)
            {
                return oldestParsedDate.Value.ToString("yyyy-dd-MM");
            }
            else
            {
                return null;
            }
        }
    }
}
