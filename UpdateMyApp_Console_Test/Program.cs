using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UpdateMyApp;

namespace UpdateMyApp_ConsoleApp_Test
{
    internal class Program
    {
        private const string CorrectXmlURL = "https://dl.dropboxusercontent.com/s/3a1x9sis8pbekhk/UpdateMyAppTemplate.xml?=0";

        private static async Task DownloadAsync()
        {
            Update.IsEnableError = true;

            Update.DownloadedProgress += Update_DownloadedProgress;

            Update.SetCurrentVersion(System.Reflection.Assembly.GetEntryAssembly().GetName().Version);

            if (Update.SetUrlToXml(CorrectXmlURL))
                if (Update.SetCurrentVersion("1.0.0"))
                    if (await Update.CheckForNewVersionAsync())
                    {
                        await Update.DownloadFileAsync(@"My_App_New.zip");
                        Update.OpenURL();
                    }

            Dictionary<string, string> AllXMLValue = await Update.ReadAllValueFromXmlAsync();
            AllXMLValue.TryGetValue("Whats_new", out string whatsNew);

            if (!string.IsNullOrEmpty(whatsNew))
                Console.WriteLine($"New version contein: {whatsNew}");
        }

        private static void Update_DownloadedProgress(long byteDownloaded, long byteToDownload, double perCentProgress)
        {
            Console.WriteLine($"Downloaded: {byteDownloaded} from: {byteToDownload} | {perCentProgress}");
        }

        private static void Main(string[] args)
        {
            DownloadAsync().Wait();
        }
    }
}