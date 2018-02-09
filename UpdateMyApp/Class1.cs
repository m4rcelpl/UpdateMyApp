using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
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


        public static async Task DownloadFileAsync(string url)
        {
            
            using (HttpResponseMessage response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result)
            {
                response.EnsureSuccessStatusCode();

                using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream("C:\\ProgramData\\TEST\\DownloadTest.zip", FileMode.Create, FileAccess.Write, FileShare.None, 1024, true))
                {
                    var totalRead = 0L;
                    var totalReads = 0L;
                    var buffer = new byte[1024];
                    var isMoreToRead = true;

                    do
                    {
                        var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            isMoreToRead = false;
                        }
                        else
                        {
                            await fileStream.WriteAsync(buffer, 0, read);

                            totalRead += read;
                            totalReads += 1;

                            if (totalReads % 2000 == 0)
                            {
                                Console.Clear();
                                Console.WriteLine(string.Format("total bytes downloaded so far: {0:n0}", totalRead));
                                
                            }
                        }
                    }
                    while (isMoreToRead);
                }
            }
        }
    }
}