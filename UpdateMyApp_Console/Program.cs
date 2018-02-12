using System;
using System.Net.Http;
using System.Threading.Tasks;
using UpdateMyApp;

namespace UpdateMyApp_Console
{
    internal class Program
    {
        private const string CorrectXmlURL = "https://dl.dropboxusercontent.com/s/3a1x9sis8pbekhk/UpdateMyAppTemplate.xml?=0";
        private const string BadXmlURL = "https://www.guugle.zn/jhbu.xml";

        private const string NewestVersion = "2.0.0";
        private const string OlderstVersion = "0.1.0";
        private const string SameVersion = "1.0.0";

        private static HttpClient client = new HttpClient();

        private static async Task DownloadAsync()
        {
            Update.DownloadedProgress += Update_DownloadedProgress;

            if (Update.SetUrlToXml(CorrectXmlURL))
                if (Update.SetCurrentVersion(OlderstVersion))
                    if (await Update.CheckForNewVersionAsync())
                    {
                        await Update.DownloadFileAsync("E:\\Shared\\TEST\\Test.zip");
                        Update.OpenURL();
                    }
        }

        private static void Update_DownloadedProgress(long byteDownloaded, long byteToDownload, double perCentProgress)
        {
            Console.WriteLine($"Ściągnięto: {byteDownloaded} z: {byteToDownload} | {perCentProgress}");
        }

        private static void Main(string[] args)
        {
            DownloadAsync().Wait();
        }
    }
}