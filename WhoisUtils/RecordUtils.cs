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
        public static string FindRecordValueStr(Dictionary<string, StringBuilder> records, string key)
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
