// -----------------------------------------------------------------------
// <copyright file="WhoisUnpack.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Download
{
    using System;
    using System.IO;
    using Ionic.BZip2;
    using Ionic.Zip;
    using Ionic.Zlib;
    using NLog;

    public class WhoisUnpack
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static bool UnpackArin(string rootPath, DateTime date)
        {
            var archivePath = Path.Combine(rootPath, "ARIN", "Raw", date.ToString("yyyy"), date.ToString("MM"), date.ToString("dd"), "arin_db.zip");
            var outPath = Path.Combine(rootPath, "ARIN", "Raw", date.ToString("yyyy"), date.ToString("MM"), date.ToString("dd"));

            return UnpackZip(archivePath, outPath, "arin_db.txt", @"\");
        }

        public static bool UnpackAfrinic(string rootPath, DateTime date)
        {
            var archivePath = Path.Combine(rootPath, "AFRINIC", "Raw", date.ToString("yyyy"), date.ToString("MM"), date.ToString("dd"), "whois_dump.bz2");
            var completeOutputPath = Path.Combine(rootPath, "AFRINIC", "Raw", date.ToString("yyyy"), date.ToString("MM"), date.ToString("dd"), "whois_dump");

            return UnpackBZip2(archivePath, completeOutputPath);
        }

        public static bool UnpackApnic(string rootPath, DateTime date)
        {
            var archivePath = Path.Combine(rootPath, "APNIC", "Raw", date.ToString("yyyy"), date.ToString("MM"), date.ToString("dd"), "apnic.RPSL.db.gz");
            var completeOutputPath = Path.Combine(rootPath, "APNIC", "Raw", date.ToString("yyyy"), date.ToString("MM"), date.ToString("dd"), "apnic.RPSL.db");

            return UnpackGZip(archivePath, completeOutputPath);
        }

        public static bool UnpackZip(string archivePath, string outPath, string selectionCriteria = null, string directoryInArchive = null)
        {
            try
            {
                using (var zip = ZipFile.Read(archivePath))
                {
                    if (selectionCriteria == null || directoryInArchive == null)
                    {
                        zip.ExtractAll(outPath);
                    }
                    else
                    {
                        zip.ExtractSelectedEntries(selectionCriteria, directoryInArchive, outPath, ExtractExistingFileAction.OverwriteSilently);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public static bool UnpackBZip2(string archivePath, string completeOutputPath)
        {
            try
            {
                var buffer = new byte[4096];

                using (var stream = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
                {
                    using (var bz2Stream = new BZip2InputStream(stream))
                    {
                        using (var outStream = File.Create(completeOutputPath))
                        {
                            var bytesRead = 0;

                            do
                            {
                                bytesRead = bz2Stream.Read(buffer, 0, buffer.Length);
                                outStream.Write(buffer, 0, bytesRead);
                            }
                            while (bytesRead != 0);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public static bool UnpackGZip(string archivePath, string completeOutputPath)
        {
            try
            {
                var buffer = new byte[4096];

                using (var stream = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
                {
                    using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        using (var outStream = File.Create(completeOutputPath))
                        {
                            var bytesRead = 0;

                            do
                            {
                                bytesRead = gzipStream.Read(buffer, 0, buffer.Length);
                                outStream.Write(buffer, 0, bytesRead);
                            }
                            while (bytesRead != 0);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
    }
}
