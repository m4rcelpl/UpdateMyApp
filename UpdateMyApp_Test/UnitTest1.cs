using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using UpdateMyApp;

namespace UpdateMyApp_Test
{
    [TestClass]
    public class UnitTest1
    {
        private const string CorrectXmlURL = "https://dl.dropboxusercontent.com/s/3a1x9sis8pbekhk/UpdateMyAppTemplate.xml";
        private const string BadXmlURL = "https://www.guugle.zn/jhbu.xml";

        private const string NewestVersion = "2.0.0";
        private const string OlderstVersion = "0.1.0";
        private const string SameVersion = "1.0.0";

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task CheckForNewVersionBadDataCorrNullAsync()
        {
            Update.IsEnableError = true;

            try
            {
                await Update.CheckForNewVersionAsync(CorrectXmlURL, null);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task CheckForNewVersionBadDataNullCurrAsync()
        {
            Update.IsEnableError = true;

            try
            {
                await Update.CheckForNewVersionAsync(null, new Version(NewestVersion));
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task CheckForNewVersionBadDataCurrEmptyAsync()
        {
            Update.IsEnableError = true;

            try
            {
                await Update.CheckForNewVersionAsync(CorrectXmlURL, new Version(""));
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task CheckForNewVersionBadDataEmptyCurrAsync()
        {
            Update.IsEnableError = true;

            try
            {
                await Update.CheckForNewVersionAsync("", new Version(NewestVersion));
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task CheckForNewVersionBadDataCurrSpaceAsync()
        {
            Update.IsEnableError = true;

            try
            {
                await Update.CheckForNewVersionAsync(CorrectXmlURL, new Version(" "));
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task CheckForNewVersionBadDataSpaceCurrAsync()
        {
            Update.IsEnableError = true;

            try
            {
                await Update.CheckForNewVersionAsync(" ", new Version(NewestVersion));
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task CheckForNewVersionTrueAsync()
        {
            Assert.IsTrue(await Update.CheckForNewVersionAsync(CorrectXmlURL, new Version(OlderstVersion)));
        }

        [TestMethod]
        public async Task CheckForNewVersionFalseAsync()
        {
            Assert.IsFalse(await Update.CheckForNewVersionAsync(CorrectXmlURL, new Version(NewestVersion)));
        }
    }
}