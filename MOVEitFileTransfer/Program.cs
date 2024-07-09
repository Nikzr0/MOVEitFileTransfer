using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace MOVEitFileTransfer
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string moveitServerUrl;

        static async Task Main(string[] args)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string localFolderPath = Path.Combine(desktopPath, "Local");

            if (!Directory.Exists(localFolderPath))
            {
                Directory.CreateDirectory(localFolderPath);
                Console.WriteLine($"Created local folder '{localFolderPath}' on the desktop.");
            }
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string username = configuration["Username"];
            string password = configuration["Password"];

            string authToken = null;
            try
            {
                authToken = await FileTransferUtility.Authenticate(username, password);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Failed to make HTTP request to the server.", ex);
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error parsing JSON response from server.", ex);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Error retrieving folder ID from response.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred.", ex);
            }

            string folderId = null;
            try
            {
                folderId = await FileTransferUtility.GetAnyFolderId(authToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            FileSystemWatcher watcher = new FileSystemWatcher(localFolderPath);

            watcher.Created += async (sender, e) =>
            {
                string newFilePath = e.FullPath;
                await FileTransferUtility.UploadFile(newFilePath, folderId, authToken);
            };

            watcher.Deleted += async (sender, e) =>
            {
                string deletedFilePath = e.FullPath;
                await FileTransferUtility.DeleteFile(deletedFilePath, authToken);
            };

            watcher.Renamed += async (sender, e) =>
            {
                string oldFilePath = e.OldFullPath;
                string newFilePath = e.FullPath;
                await FileTransferUtility.RenameFile(oldFilePath, newFilePath, authToken);
            };

            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Monitoring local folder '{localFolderPath}' for new files. Press any key to exit...");
            Console.ReadKey();
        }
    }
}