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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string localFolderPath = configuration["LocalFolderPath"];

            if (string.IsNullOrEmpty(localFolderPath))
            {
                Console.WriteLine("LocalFolderPath is not configured in appsettings.json.");
                return;
            }

            moveitServerUrl = configuration["MoveitServerUrl"];
            string username = configuration["Username"];
            string password = configuration["Password"];

            string authToken = null;
            try
            {
                authToken = await Authenticate(username, password);
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
                folderId = await GetAnyFolderId(authToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            FileSystemWatcher watcher = new FileSystemWatcher(localFolderPath);

            watcher.Created += async (sender, e) =>
            {
                string newFilePath = e.FullPath;
                await UploadFile(newFilePath, folderId, authToken);
            };

            watcher.Deleted += async (sender, e) =>
            {
                string deletedFilePath = e.FullPath;
                await DeleteFile(deletedFilePath, authToken);
            };

            watcher.Renamed += async (sender, e) =>
            {
                string oldFilePath = e.OldFullPath;
                string newFilePath = e.FullPath;
                await RenameFile(oldFilePath, newFilePath, authToken);
            };

            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Monitoring local folder '{localFolderPath}' for new files. Press any key to exit...");
            Console.ReadKey();
        }

        public static async Task<string> Authenticate(string username, string password)
        {
            string authUrl = $"{moveitServerUrl}/token";
            var requestData = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "username", username },
            { "password", password }
        };

            HttpResponseMessage response = await client.PostAsync(authUrl, new FormUrlEncodedContent(requestData));
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonDocument.Parse(responseContent).RootElement;

            try
            {
                if (responseData.TryGetProperty("access_token", out var accessToken))
                {
                    return accessToken.GetString();
                }
                else
                {
                    throw new InvalidOperationException("Access token not found in the response.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse access token from the response - {ex.Message}");
                return null;
            }
        }

        public static async Task<string> GetAnyFolderId(string authToken)
        {
            string foldersUrl = $"{moveitServerUrl}/folders";

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");

            HttpResponseMessage response = await client.GetAsync(foldersUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var foldersResponse = JsonDocument.Parse(responseContent).RootElement;

            if (foldersResponse.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array && items.GetArrayLength() > 0)
            {
                foreach (var folder in items.EnumerateArray())
                {
                    if (folder.TryGetProperty("permission", out var permission))
                    {
                        if (permission.TryGetProperty("canWriteFiles", out var canWriteFiles) && canWriteFiles.GetBoolean())
                        {
                            if (folder.TryGetProperty("id", out var id))
                            {
                                return id.ToString();
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException("No folders found with write permissions or invalid folder ID.");
        }

        public static async Task UploadFile(string filePath, string folderId, string authToken)
        {
            string uploadUrl = $"{moveitServerUrl}/folders/{folderId}/files";
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");

            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StreamContent(new FileStream(filePath, FileMode.Open)), "file", Path.GetFileName(filePath));

                HttpResponseMessage response = await client.PostAsync(uploadUrl, formData);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Uploaded '{Path.GetFileName(filePath)}' to MOVEit Transfer.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"The file '{Path.GetFileName(filePath)}' already exists in MOVEit Transfer.");
                }
                else
                {
                    Console.WriteLine($"Failed to upload '{Path.GetFileName(filePath)}'.");
                                                                                           
                                                                                           
                }
            }
        }

        public static async Task DeleteFile(string localFileToRemove, string authToken)
        {
            try
            {
                string fileName = Path.GetFileName(localFileToRemove);

                string fileId = await GetFileId(fileName, authToken);

                if (fileId != null)
                {
                    string deleteUrl = $"{moveitServerUrl}/files/{fileId}";

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");

                    HttpResponseMessage response = await client.DeleteAsync(deleteUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Deleted '{fileName}' from MOVEit Transfer.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to delete '{fileName}' from MOVEit Transfer.");
                    }
                }
                else
                {
                    Console.WriteLine($"File '{fileName}' not found in MOVEit Transfer.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file from MOVEit Transfer - {ex.Message}");
            }
        }

        public static async Task<string> GetFileId(string fileName, string authToken)
        {
            string filesUrl = $"{moveitServerUrl}/files";

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");

            HttpResponseMessage response = await client.GetAsync(filesUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var filesResponse = JsonDocument.Parse(responseContent).RootElement;

            if (filesResponse.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array && items.GetArrayLength() > 0)
            {
                foreach (var file in items.EnumerateArray())
                {
                    if (file.TryGetProperty("name", out var name) && name.GetString() == fileName)
                    {
                        if (file.TryGetProperty("id", out var id))
                        {
                            return id.ToString();
                        }
                    }
                }
            }

            return null;
        }

        public static async Task RenameFile(string oldFilePath, string newFilePath, string authToken)
        {
            try
            {
                string oldFileName = Path.GetFileName(oldFilePath);
                string newFileName = Path.GetFileName(newFilePath);

                string fileId = await GetFileId(oldFileName, authToken);

                if (fileId != null)
                {
                    string renameUrl = $"{moveitServerUrl}/files/{fileId}";

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");

                    var renameData = new Dictionary<string, string>
                    {
                        { "name", newFileName }
                    };

                    var content = new StringContent(JsonSerializer.Serialize(renameData), System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PatchAsync(renameUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Renamed '{oldFileName}' to '{newFileName}' in MOVEit Transfer.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to rename '{oldFileName}' to '{newFileName}' in MOVEit Transfer.");
                    }
                }
                else
                {
                    Console.WriteLine($"File '{oldFileName}' not found in MOVEit Transfer.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error renaming file in MOVEit Transfer - {ex.Message}");
            }
        }
    }
}