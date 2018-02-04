using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using UpdateMyApp;

namespace UpdateMyApp_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task CheckForNewVersionBadDataAsync()
        {
            Update.IsEnableError = true;

            try
            {
                await Update.CheckForNewVersionAsync(null, null);
                Assert.Fail("Expect exception [1]");

                await Update.CheckForNewVersionAsync("https://dl.dropboxusercontent.com/s/mpfyioal1oxbs6v/KeyHolderUpdate.xml", null);
                Assert.Fail("Expect exception [2]");

                await Update.CheckForNewVersionAsync(null, "1.4.4");
                Assert.Fail("Expect exception [3]");

                await Update.CheckForNewVersionAsync("", "");
                Assert.Fail("Expect exception [4]");

                await Update.CheckForNewVersionAsync("https://dl.dropboxusercontent.com/s/mpfyioal1oxbs6v/KeyHolderUpdate.xml", "");
                Assert.Fail("Expect exception [5]");

                await Update.CheckForNewVersionAsync("", "1.4.4");
                Assert.Fail("Expect exception [6]");

                await Update.CheckForNewVersionAsync(" ", " ");
                Assert.Fail("Expect exception [7]");

                await Update.CheckForNewVersionAsync("https://dl.dropboxusercontent.com/s/mpfyioal1oxbs6v/KeyHolderUpdate.xml", " ");
                Assert.Fail("Expect exception [8]");

                await Update.CheckForNewVersionAsync(" ", "1.4.4");
                Assert.Fail("Expect exception [9]");
            }
            catch (Exception)
            {
            }
            
        }

        [TestMethod]
        public async Task CheckForNewVersionTrueAsync()
        {
            Assert.IsTrue(await Update.CheckForNewVersionAsync("https://dl.dropboxusercontent.com/s/mpfyioal1oxbs6v/KeyHolderUpdate.xml", "1.4.0"));
        }

        [TestMethod]
        public async Task CheckForNewVersionFalseAsync()
        {
            Assert.IsFalse(await Update.CheckForNewVersionAsync("https://dl.dropboxusercontent.com/s/mpfyioal1oxbs6v/KeyHolderUpdate.xml", "1.4.5"));
        }

    }
}