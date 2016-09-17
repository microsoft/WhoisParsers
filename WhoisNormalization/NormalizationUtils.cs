// -----------------------------------------------------------------------
// <copyright file="NormalizationUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Parsers;

    public static class NormalizationUtils
    {
        private static string[] dateFormats = { "yyyy-MM-dd", "yyyyMMdd" };
        private static string dateOutputFormat = "yyyy-MM-dd";
        private static char[] dateWordsSplitChars = new char[] { ' ', '\t', '\r', '\n' };
        private static CultureInfo dateCultureInfo = new CultureInfo("en-US");

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
            target.Created = FindOldestDateOptimized(FindFirstMatchingFieldValueInRecords(section, createdFields));
            target.Updated = FindOldestDateOptimized(FindFirstMatchingFieldValueInRecords(section, updatedFields));
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

        public static DateTime? ExtractDateExact(string text)
        {
            if (text == null)
            {
                return null;
            }

            var dateNoDash = ExtractDateExactNoDash(text);
            var dateDash = ExtractDateExactDash(text);

            if (dateNoDash != null)
            {
                return dateNoDash;
            }

            if (dateDash != null)
            {
                return dateDash;
            }

            return null;
        }

        public static string FindOldestDateOptimized(string text)
        {
            if (text == null)
            {
                return null;
            }

            var words = new List<string>(text.Split(dateWordsSplitChars));
            words = words.Select(word => word.Trim()).ToList<string>();
            words = words.Where(word => word.Length > 0).ToList<string>();

            DateTime? oldestParsedDate = null;

            foreach (var word in words)
            {
                DateTime? currentParsedDate = ExtractDateExact(word);

                if (currentParsedDate != null)
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
                return oldestParsedDate.Value.ToString(dateOutputFormat);
            }
            else
            {
                return null;
            }
        }

        public static string FindOldestDateSlow(string text)
        {
            if (text == null)
            {
                return null;
            }

            var words = new List<string>(text.Split(dateWordsSplitChars));
            words = words.Select(word => word.Trim()).ToList<string>();
            words = words.Where(word => word.Length > 0).ToList<string>();

            DateTime? oldestParsedDate = null;

            foreach (var word in words)
            {
                DateTime currentParsedDate;

                if (DateTime.TryParseExact(s: word, formats: dateFormats, provider: dateCultureInfo, style: DateTimeStyles.None, result: out currentParsedDate))
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
                return oldestParsedDate.Value.ToString(dateOutputFormat);
            }
            else
            {
                return null;
            }
        }

        private static DateTime? ExtractDateExactNoDash(string text)
        {
            // Example: 20101112
            if (text.Length != 8)
            {
                return null;
            }

            foreach (var c in text)
            {
                if (!char.IsNumber(c))
                {
                    return null;
                }
            }

            if (text[0] == '0')
            {
                return null;
            }

            var rawYear = text.Substring(0, 4);
            var rawMonth = text.Substring(4, 2);
            var rawDay = text.Substring(6, 2);

            int year;
            int month;
            int day;

            if (!int.TryParse(rawYear, out year))
            {
                return null;
            }

            if (!int.TryParse(rawMonth, out month))
            {
                return null;
            }

            if (!int.TryParse(rawDay, out day))
            {
                return null;
            }

            return new DateTime(year, month, day);
        }

        private static DateTime? ExtractDateExactDash(string text)
        {
            // Example: 2010-11-12
            if (text.Length != 10)
            {
                return null;
            }

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                switch (i)
                {
                    case 4: // First dash
                    case 7: // Second dash
                        if (c != '-')
                        {
                            return null;
                        }

                        break;
                    default:

                        if (!char.IsNumber(c))
                        {
                            return null;
                        }

                        break;
                }
            }

            if (text[0] == '0')
            {
                return null;
            }

            var rawYear = text.Substring(0, 4);
            var rawMonth = text.Substring(5, 2);
            var rawDay = text.Substring(8, 2);

            int year;
            int month;
            int day;

            if (!int.TryParse(rawYear, out year))
            {
                return null;
            }

            if (!int.TryParse(rawMonth, out month))
            {
                return null;
            }

            if (!int.TryParse(rawDay, out day))
            {
                return null;
            }

            return new DateTime(year, month, day);
        }
    }
}
