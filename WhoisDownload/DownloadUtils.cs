// -----------------------------------------------------------------------
// <copyright file="DownloadUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Download
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using NLog;

    public static class DownloadUtils
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static async Task<bool> DownloadFile(string url, string completeOutPath, string username = null, string password = null)
        {
            try
            {
                using (var client = new WebClient())
                {
                    if (username != null && password != null)
                    {
                        client.Credentials = new NetworkCredential(username, password);
                    }

                    await client.DownloadFileTaskAsync(new Uri(url), completeOutPath);

                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
    }
}
