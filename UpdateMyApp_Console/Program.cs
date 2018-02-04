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
        private static HttpClient client = new HttpClient();

        private static async Task DownloadAsync()
        {
        }

        private static void Main(string[] args)
        {
            //DownloadAsync().Wait();

            var test = Update.CheckForNewVersionAsync("https://dl.dropboxusercontent.com/s/mpfyioal1oxbs6v/KeyHolderUpdate.xml", "1.4.3").Result;
            var test2 = Update.CheckForNewVersionAsync("https://dl.dropboxusercontent.com/s/mpfyioal1oxbs6v/KeyHolderUpdate.xml", "1.4.5").Result;
            var test3 = Update.ReadAllValueFromXml("https://dl.dropboxusercontent.com/s/mpfyioal1oxbs6v/KeyHolderUpdate.xml").Result;


            Console.WriteLine("Hello World!");
        }
    }
}