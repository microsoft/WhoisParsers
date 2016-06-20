// -----------------------------------------------------------------------
// <copyright file="TextUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Utils
{
    using System.Collections.Generic;
    using System.IO;

    public static class TextUtils
    {
        public static List<string> SplitTextToLines(string text, bool removeEmptyEntries)
        {
            var ret = new List<string>();

            using (var sr = new StringReader(text))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (!removeEmptyEntries || line.Length > 0)
                    {
                        ret.Add(line);
                    }
                }
            }

            return ret;
        }
    }
}
