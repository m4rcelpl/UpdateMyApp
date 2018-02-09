using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using UpdateMyApp;

namespace UpdateMyApp_Console
{
    internal class Program
    {


        private const string CorrectXmlURL = "https://dl.dropboxusercontent.com/s/3a1x9sis8pbekhk/UpdateMyAppTemplate.xml";
        private const string BadXmlURL = "https://www.guugle.zn/jhbu.xml";

        private const string NewestVersion = "2.0.0";
        private const string OlderstVersion = "0.1.0";
        private const string SameVersion = "1.0.0";

        private static HttpClient client = new HttpClient();

        private static async Task DownloadAsync()
        {
            await Update.DownloadFileAsync("https://dl.dropboxusercontent.com/s/fmn3vswihogd2xj/FRITZ15.zip");

            try
            {
                var test5 = await Update.CheckForNewVersionAsync(null, null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void Main(string[] args)
        {

            DownloadAsync().Wait();


            Update.IsEnableError = true;

            //var test = Update.CheckForNewVersionAsync(CorrectXmlURL, new Version("0.1.0")).Result;
            //var test2 = Update.CheckForNewVersionAsync(CorrectXmlURL, new Version("2.0.0")).Result;

            //var test5 = Update.CheckForNewVersionAsync(null, new Version("2.0.0")).Result;

            //var test3 = Update.ReadAllValueFromXml(CorrectXmlURL).Result;

            //bool isUri = Uri.IsWellFormedUriString(CorrectXmlURL, UriKind.RelativeOrAbsolute);
            //bool isUri1 = Uri.IsWellFormedUriString(BadXmlURL, UriKind.RelativeOrAbsolute);
            //bool isUri2 = Uri.IsWellFormedUriString("www.dl.dropboxusercontent.com/s/3a1x9sis8pbekhk/UpdateMyAppTemplate.xml", UriKind.RelativeOrAbsolute);
            //bool isUri3 = Uri.IsWellFormedUriString("jhb.iu.iyu.", UriKind.RelativeOrAbsolute);

            


            Console.WriteLine("Hello World!");
        }
    }
}