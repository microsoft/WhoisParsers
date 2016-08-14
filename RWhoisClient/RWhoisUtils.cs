// -----------------------------------------------------------------------
// <copyright file="RWhoisUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Client
{
    using System;
    using System.Collections.Generic;

    public static class RWhoisUtils
    {
        public static bool IsOkResponse(List<string> responseLines)
        {
            if (responseLines != null && responseLines.Count > 0 && responseLines[responseLines.Count - 1].Trim() == "%ok")
            {
                return true;
            }

            return false;
        }
    }
}
