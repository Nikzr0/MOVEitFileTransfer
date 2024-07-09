using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MOVEitFileTransfer.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        private IConfigurationRoot configuration;

        [SetUp]
        public void Setup()
        {
            string basePath = TestContext.CurrentContext.TestDirectory;
            string appsettingsPath = Path.Combine(basePath, "..","..", "..", "..", "MOVEitFileTransfer", "appsettings.json");
            configuration = new ConfigurationBuilder()
                .AddJsonFile(appsettingsPath, optional: false, reloadOnChange: true)
                .Build();
        }

        [Test]
        public void TestLocalFolderPath()
        {
            string localFolderPath = configuration["LocalFolderPath"];
            Assert.That(localFolderPath, Is.Not.Null.And.Not.Empty, "LocalFolderPath should not be empty.");
            Assert.That(Directory.Exists(localFolderPath), Is.True, $"Local folder path '{localFolderPath}' does not exist.");
        }

        [Test]
        public void TestMoveitServerUrl()
        {
            string moveitServerUrl = configuration["MoveitServerUrl"];
            Assert.That(moveitServerUrl, Is.Not.Null.And.Not.Empty, "MoveitServerUrl should not be empty.");
        }

        [Test]
        public void TestUsername()
        {
            string username = configuration["Username"];
            Assert.That(username, Is.Not.Null.And.Not.Empty, "Username should not be empty.");
        }

        [Test]
        public void TestPassword()
        {
            string password = configuration["Password"];
            Assert.That(password, Is.Not.Null.And.Not.Empty, "Password should not be empty.");
        }
    }
}