// -----------------------------------------------------------------------
// <copyright file="RecordUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Utils
{
    using System.Collections.Generic;
    using System.Text;

    public static class RecordUtils
    {
        public static string FindFirstValueForKeys(Dictionary<string, StringBuilder> records, List<string> keys)
        {
            foreach (var key in keys)
            {
                var value = FindValueForKey(records, key);

                if (value != null)
                {
                    return value;
                }
            }

            return null;
        }

        public static string FindValueForKey(Dictionary<string, StringBuilder> records, string key)
        {
            StringBuilder value;

            if (records.TryGetValue(key, out value))
            {
                return value.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
