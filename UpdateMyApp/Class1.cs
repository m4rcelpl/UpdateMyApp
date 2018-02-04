using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UpdateMyApp
{
    public class Update
    {
        public static bool IsEnableError { get; set; } = false;
        private static readonly HttpClient client = new HttpClient();



        /// <summary>
        /// Check from xml if new version is ready.
        /// </summary>
        /// <param name="XmlURL"></param>
        /// <returns></returns>
        public static async Task<bool> CheckForNewVersionAsync(string XmlURL, string CurrentAppVersion)
        {
            
            Version VersionCurrent = new Version(CurrentAppVersion);
            Version VersionFromXml = new Version();


            if (string.IsNullOrEmpty(XmlURL) || string.IsNullOrWhiteSpace(XmlURL))
            {
                if (IsEnableError)
                    throw new Exception($"[UpdateMyApp][{DateTime.Now}] XmlURL is empty or null");
                else
                    Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] XmlURL is empty or null");
            }
            else if(string.IsNullOrEmpty(CurrentAppVersion) || string.IsNullOrWhiteSpace(CurrentAppVersion))
            {
                if (IsEnableError)
                    throw new Exception($"[UpdateMyApp][{DateTime.Now}] CurrentAppVersion is empty or null");
                else
                    Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] CurrentAppVersion is empty or null");
            }

            Dictionary<string, string> _dictopnery = await ReadXmlFromURL(XmlURL);

            if (_dictopnery.Count == 0)
                return false;
            
            if(_dictopnery.ContainsKey("version"))
            {
                _dictopnery.TryGetValue("version", out string _version);
                VersionFromXml = new Version(_version);

                if(VersionCurrent < VersionFromXml)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (IsEnableError)
                    throw new Exception($"[UpdateMyApp][{DateTime.Now}] XML do not contain 'version' element");
                else
                    Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] XML do not contain 'version' element");
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
                    if(ex.InnerException != null)
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] {ex.InnerException.Message}");
                    else
                        Debug.WriteLine($"[UpdateMyApp][{DateTime.Now}] {ex.Message}");
                }
                    
            }

            return _dictopnery;
        }

        public static async Task<Dictionary<string, string>> ReadAllValueFromXml(string XmlURL)
        {
            return await ReadXmlFromURL(XmlURL);
        }

    }
}