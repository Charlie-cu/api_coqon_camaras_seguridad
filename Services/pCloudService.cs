using System.Collections.Generic;
using Newtonsoft.Json.Linq;
namespace api_coqon.Services
{
    public class pCloudService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;

        public pCloudService(string accessToken)
        {
            _httpClient = new HttpClient();
            _accessToken = accessToken;
        }

        public async Task<bool> UploadStreamAsync(Stream videoStream, string fileName)
        {
            var url = $"https://api.pcloud.com/uploadfile";

            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(videoStream), "file", fileName);
            content.Add(new StringContent(_accessToken), "access_token");

            var response = await _httpClient.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }

        internal async Task<bool> UploadStreamAsync(string fileName, Stream stream)
        {
            throw new NotImplementedException();
        }
        public async Task DeleteOldFilesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://api.pcloud.com/listfolder?access_token={_accessToken}&folderid=0");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var files = ParseFilesFromResponse(content); // Implementa esta función para analizar los archivos

                foreach (var file in files)
                {
                    var fileDate = ExtractDateFromFileName(file.Name); // Implementa para extraer la fecha del nombre
                    if (fileDate != null && (DateTime.UtcNow - fileDate.Value).TotalHours > 72)
                    {
                        await _httpClient.GetAsync($"https://api.pcloud.com/deletefile?access_token={_accessToken}&fileid={file.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error deleting old files: {ex.Message}");
            }
        }
        private DateTime? ExtractDateFromFileName(string fileName)
        {
            try
            {
                var parts = fileName.Split('_');
                if (parts.Length >= 3 && DateTime.TryParseExact(parts[2], "yyyyMMdd_HHmmss", null, System.Globalization.DateTimeStyles.None, out var date))
                {
                    return date;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public List<FileInfo> ParseFilesFromResponse(string jsonResponse)
        {
            var fileList = new List<FileInfo>();

            try
            {
                var json = JObject.Parse(jsonResponse);
                var files = json["metadata"]?["contents"];

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file["isfolder"]?.Value<bool>() == false) // Asegura que solo se procesen archivos
                        {
                            fileList.Add(new FileInfo
                            {
                                Id = file["fileid"]?.Value<string>(),
                                Name = file["name"]?.Value<string>()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error parsing files from response: {ex.Message}");
            }

            return fileList;
        }

        public class FileInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
