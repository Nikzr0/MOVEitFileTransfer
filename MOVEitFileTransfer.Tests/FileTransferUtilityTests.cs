namespace MOVEitFileTransfer.Tests
{
    [TestFixture]
    public class FileTransferUtilityTests
    {
        private string _testFilePath;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), "test.txt");
            File.WriteAllText(_testFilePath, "Test file content");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }

        [Test]
        public async Task UploadFile_FileExists_Success()
        {
            string authToken = "<valid_auth_token>";

            await FileTransferUtility.UploadFile(_testFilePath, "<valid_folder_id>", authToken);
        }

        [Test]
        public void UploadFile_FileDoesNotExist_ThrowsException()
        {
            string authToken = "<valid_auth_token>";
            Assert.ThrowsAsync<FileNotFoundException>(async () => await FileTransferUtility.UploadFile("non_existing_file.txt", "<valid_folder_id>", authToken));
        }
    }
}
