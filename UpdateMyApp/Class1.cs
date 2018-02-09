using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UpdateMyApp
{
    public class Update
    {
        /// <summary>
        /// If you set to true you must catch all error. False by default.
        /// </summary>
        public static bool IsEnableError { get; set; } = false;

        private static readonly HttpClient client = new HttpClient();

        public delegate void DownloadedProgressDelegate(Int64 byteDownloaded, Int64 byteToDownload, double perCentProgress);

        /// <summary>
        /// Subscribe to get byteDownloaded, byteToDownload and perCentProgress.
        /// </summary>
        public static event DownloadedProgressDelegate DownloadedProgress;

        /// <summary>
        /// Check from xml if new version is ready.
        /// </summary>
        /// <param name="XmlURL">URL to XML file</param>
        /// <param name="CurrentAppVersion">Current version</param>
        /// <returns></returns>
        public static async Task<bool> CheckForNewVersionAsync(string XmlURL, Version CurrentAppVersion)
        {
            if (string.IsNullOrEmpty(XmlURL) || string.IsNullOrWhiteSpace(XmlURL) || CurrentAppVersion == null)
            {
                if (IsEnableError)
                    throw new NullReferenceException("XmlURL is empty or null");
                else
                {
                    Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] XmlURL or CurrentAppVersion is empty or null");
                    return false;
                }
            }

            try
            {
                Dictionary<string, string> _dictopnery = await ReadXmlFromURL(XmlURL);

                if (_dictopnery.Count == 0)
                    return false;

                if (_dictopnery.ContainsKey("version"))
                {
                    _dictopnery.TryGetValue("version", out string _version);
                    Version VersionFromXml = new Version(_version);

                    if (CurrentAppVersion < VersionFromXml)
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (IsEnableError)
                        throw new Exception($"XML do not contain 'version' element");
                    else
                    {
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] XML do not contain 'version' element");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                if (IsEnableError)
                    throw;
                else
                {
                    if (ex.InnerException != null)
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] {ex.InnerException.Message}");
                    else
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] {ex.Message}");
                }
            }
            return false;
        }

        private static async Task<Dictionary<string, string>> ReadXmlFromURL(string XmlURL)
        {
            Dictionary<string, string> _dictopnery = new Dictionary<string, string>();

            try
            {
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true
                };

                var getXml = await client.GetAsync(XmlURL);
                var stream = await getXml.Content.ReadAsStreamAsync();
                var itemXml = XElement.Load(stream);

                foreach (var item in itemXml.Elements())
                {
                    _dictopnery.Add(item.Name.LocalName, item.Value);
                }

                return _dictopnery;
            }
            catch (Exception ex)
            {
                if (IsEnableError)
                    throw;
                else
                {
                    if (ex.InnerException != null)
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] {ex.InnerException.Message}");
                    else
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] {ex.Message}");
                }
            }
            return _dictopnery;
        }

        /// <summary>
        /// Reed all data from you XML.
        /// </summary>
        /// <param name="XmlURL">URL to XML file</param>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> ReadAllValueFromXml(string XmlURL)
        {
            return await ReadXmlFromURL(XmlURL);
        }

        private static async Task<Int64> GetFileSizeAsync(Uri uriPath)
        {
            try
            {
                var webRequest = HttpWebRequest.Create(uriPath);
                webRequest.Method = "HEAD";

                using (var webResponse = await webRequest.GetResponseAsync())
                {
                    return Convert.ToInt64(webResponse.Headers.Get("Content-Length"));
                }
            }
            catch (Exception ex)
            {
                if (IsEnableError)
                    throw;
                else
                {
                    if (ex.InnerException != null)
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] {ex.InnerException.Message}");
                    else
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] {ex.Message}");

                    return 0;
                }
            }
        }

        private static void EventDownloadedProgress(Int64 send, Int64 total)
        {
            double dProgress = ((double)send / total) * 100.0;
            DownloadedProgress(send, total, dProgress);
        }


        /// <summary>
        /// Download file from url. You can subscribe DownloadedProgress event do get progress. 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="destinationPatch"></param>
        /// <returns></returns>
        public static async Task<bool> DownloadFileAsync(string url, string destinationPatch)
        {
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                {
                    using (Stream streamToWriteTo = File.Open(destinationPatch, FileMode.Create))
                    {
                        Int64 totalRead = 0L;
                        var buffer = new byte[65536];
                        var isMoreToRead = true;

                        var FileSize = await GetFileSizeAsync(new Uri(url));

                        do
                        {
                            var read = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                await streamToWriteTo.WriteAsync(buffer, 0, read);

                                totalRead += read;

                                EventDownloadedProgress(totalRead, FileSize);
                            }
                        }
                        while (isMoreToRead);

                        if (totalRead == FileSize)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (IsEnableError)
                    throw;
                else
                {
                    if (ex.InnerException != null)
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] {ex.InnerException.Message}");
                    else
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] {ex.Message}");

                    return false;
                }
            }
        }
    }
}