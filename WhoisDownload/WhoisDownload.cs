// -----------------------------------------------------------------------
// <copyright file="WhoisDownload.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Download
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using NLog;

    public class WhoisDownload
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static async Task<bool> DownloadArin(string rootPath, string apiKey, DateTime? date = null)
        {
            var downloadUrlFormat = "https://www.arin.net/public/secure/downloads/bulkwhois?apikey={0}";
            var url = string.Format(CultureInfo.InvariantCulture, downloadUrlFormat, apiKey);

            return await DownloadWhoisFile(url, rootPath, outFilename: "arin_db.zip", whoisFolderName: "ARIN", date: date);
        }

        public static async Task<bool> DownloadAfrinic(string rootPath, string username, string password, DateTime? date = null)
        {
            var url = "ftp://ftp.afrinic.net/dump/whois_dump.bz2";

            return await DownloadWhoisFile(url, rootPath, outFilename: "whois_dump.bz2", whoisFolderName: "AFRINIC", date: date, username: username, password: password);
        }

        public static async Task<bool> DownloadApnic(string rootPath, string username, string password, DateTime? date = null)
        {
            var url = "ftp://ftp.apnic.net/pub/whois-data/APNIC/apnic.RPSL.db.gz";

            return await DownloadWhoisFile(url, rootPath, outFilename: "apnic.RPSL.db.gz", whoisFolderName: "APNIC", date: date, username: username, password: password);
        }

        public static async Task<bool> DownloadLacnic(string rootPath, string username, string password, DateTime? date = null)
        {
            /*
            var downloadUrlFormat = "https://lacnic.net/cgi-bin/lacnic/bulkWhoisLoader?stkey={0}&lg=EN";
            var url = string.Format(CultureInfo.InvariantCulture, downloadUrlFormat, apiKey);

            return await DownloadWhoisFile(url, rootPath, outFilename: "lacnic.db", whoisFolderName: "LACNIC", date: date);
            */
            var lacnicRootUrl = new Uri("https://lacnic.net/");
            var loginPageUrl = "https://lacnic.net/cgi-bin/lacnic/stini?lg=EN";

            using (var client = new WebClient())
            {
                var loginPageHtml = await client.DownloadStringTaskAsync(loginPageUrl);

                var loginPageDoc = new HtmlDocument();
                loginPageDoc.LoadHtml(loginPageHtml);

                var formNode = loginPageDoc.DocumentNode.SelectSingleNode("//form");

                if (formNode != null)
                {
                    var relativeFormUrl = formNode.Attributes["action"].Value;
                    var absoluteFormUrl = new Uri(lacnicRootUrl, relativeFormUrl);

                    var loginValues = new NameValueCollection();
                    loginValues.Add("handle", username);
                    loginValues.Add("passwd", password);

                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    var bulkWhoisPageBytes = await client.UploadValuesTaskAsync(absoluteFormUrl, "POST", loginValues);
                    var bulkWhoisPageHtml = Encoding.UTF8.GetString(bulkWhoisPageBytes);

                    var bulkWhoisPageDoc = new HtmlDocument();
                    bulkWhoisPageDoc.LoadHtml(bulkWhoisPageHtml);

                    var bulkLinkNode = bulkWhoisPageDoc.DocumentNode.SelectSingleNode("//li/a[1]");

                    if (bulkLinkNode != null)
                    {
                        var relativeBulkLinkUrl = bulkLinkNode.Attributes["href"].Value;
                        var absoluteBulkLinkUrl = new Uri(lacnicRootUrl, relativeBulkLinkUrl);

                        return await DownloadWhoisFile(absoluteBulkLinkUrl.ToString(), rootPath, outFilename: "lacnic.db", whoisFolderName: "LACNIC", date: date);
                    }
                }
            }

            return false;
        }

        public static async Task<bool> DownloadWhoisFile(string url, string rootPath, string outFilename, string whoisFolderName, DateTime? date = null, string username = null, string password = null)
        {
            if (date == null)
            {
                date = DateTime.UtcNow;
            }

            var outPath = Path.Combine(rootPath, whoisFolderName, "Raw", date.Value.ToString("yyyy"), date.Value.ToString("MM"), date.Value.ToString("dd"));

            try
            {
                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }

                var completeOutPath = Path.Combine(outPath, outFilename);

                return await DownloadUtils.DownloadFile(url, completeOutPath, username, password);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
    }
}
