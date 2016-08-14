// -----------------------------------------------------------------------
// <copyright file="NormalizationUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Parsers;

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

        public static string FindFirstMatchingFieldValueInRecords(RawWhoisSection section, HashSet<string> fields)
        {
            if (section != null && fields != null)
            {
                foreach (var field in fields)
                {
                    StringBuilder nameBuilder;

                    if (section.Records.TryGetValue(field, out nameBuilder))
                    {
                        return nameBuilder.ToString();
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
            target.Name = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, nameFieldNames);
            target.Created = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, createdFields);
            target.Updated = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, updatedFields);
            target.UpdatedBy = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, updatedByFields);
            target.Description = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, descriptionFields);
            target.Comment = NormalizationUtils.FindFirstMatchingFieldValueInRecords(section, commentFields);
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
    }
}
