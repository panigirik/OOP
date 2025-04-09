using System.Net.Http.Headers;

namespace ConsoleWord.Application.Dropbox
{
    public class DropboxStorageProvider : ICloudDropBoxStorageProvider
    {
        private const string AccessToken = "sl.u.AFqaBDYEnGUmAK1YOjrpkwq5Z3T24TQ4n7etriSWzCxe9yWoPfid7x-gDC9rBwbs-sKQz14TvvKiCCCsr4IJhsjByESftE-zIUktQAEyfWSJJirLu2mckCLWK4n1LhLeh7OO7J2EgtaOx3BTe93jExJnXqyNDVng9JkO666AE4xBHCTiZ9lkQiexPtzFDBekF0bPDBOr4IRW7T3BTd6II23WV8-J7FHY4hu5QqGgkQvif9if98inD6SZbS0M8taFpMbqi031IoFPJvz2lsd3a9mo7h1imRRg0f2zmvUVSeV1QcxEUetVpglBRbyGUyTAnEJ5nsgH1BSAcdFH3fDas37t9yIqB6nZ_Z_Nx0AM9h43pFRSWYO_fvEuSU8uCS1RJ-OTsLSizuk4UaXcCCyrLFu9CPgBswnr1XylSC1Dp_N70jB6ZJu7tpbyHGOuz2OKcEZ6h5KACAC4NH3btdZykjr1BNzzZ_qfHUxb2Q4lZc_jztmynC-MibJv7s0pMYhTXCon7Kv2iovBPu1k3cO78u1LUYYLyBnnNiJ5NN4xYQN63ezqMTuAAuAl15xFPP3LHcyMlMYYg4SxNGStfOnCWutfN0nGxOonSW7BTKI8a8gBRxjpV1bL-oOwjMh0r4L2d8EyrZuBD10K7_VstKMBxH77KojCm6UHZv0ejHu9bicjgdR3JI5wf44KJkC9oL0850BVqjjc3NaIvI-_4RnoygORw61PrOU8dwUhLc0zng-kqco9td0St9Cx299kOCwffH6M2bbF9B76t2kyiF3T76shRJhAhU6DYswlC4hY8L9bA2WEls3DZgcLKZSuqq3Z0bFe4UqifOfns9XkdZl7Gjh9ul0SpB1UISzzlDlwhi4_H5DFM2V1FuCWFDzrQ2vruV4501Y1oVexdMKqEuCFCWoPCHruPZN8TPVXddAKj1kfhz6f7FqkK9e96JfFNO-njzD7Us-Fxt9qS6MmsOKozvbMwpMhsrLVzlpthx2-O2sKauPUCwSVb8bVM9Jts9WLDVkFxjYXDhbuGfNaWCqqaV6Xa2AKC_Lt-mHLBWlSsEMHFB7l-VU0nFeNXo_WzVsZkGhHDjq7T93GPvqwGOGsvXsDlMpDxwS3nv62Qubs-cDsQoUY5aYHbdSVns2vFLmroBOuJJ5kCxp8CVSDkz8o0TMZraAnPiuTq1JW18zU7Y_X9vrHT8Z5i2iJ6UXTUQMiL8OuqY6x4shZ_q18nxf_NMFPQs_qyKtLrHC5q1mmA6rYumsT5qZGWdYswfY5MT6L4EH2GGffRw4QGi_TDvXYAtRjCgcWqOtTZe_YKsVbdDk5a7u08OhT93EqivBtt3Gc1kHs6QEjD4VeIPVxPbrWMP8XLtJNcQCsVk3KaFsZbEG2msL8ip1ysITSbVnTtz6c-wLAkdLwqTEgBsNYVP1WiqgUd5YgKlnrC3fqKYrRSD1aaQ"; // Укорочено для читаемости
        private readonly HttpClient _httpClient;

        public DropboxStorageProvider()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }

        public async Task UploadFileAsync(string fileName, byte[] fileContent)
        {
            var uploadUrl = "https://content.dropboxapi.com/2/files/upload";
            var folderPath = "/Documents"; // Без последнего '/'

            using (var content = new ByteArrayContent(fileContent))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var dropboxArg = new
                {
                    path = $"{folderPath}/{fileName}", // гарантировано один '/'
                    mode = "overwrite",
                    autorename = true,
                    mute = false
                };

                // Удаляем старый заголовок, если был
                if (_httpClient.DefaultRequestHeaders.Contains("Dropbox-API-Arg"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Dropbox-API-Arg");
                }

                _httpClient.DefaultRequestHeaders.Add(
                    "Dropbox-API-Arg",
                    Newtonsoft.Json.JsonConvert.SerializeObject(dropboxArg)
                );

                var response = await _httpClient.PostAsync(uploadUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Файл '{fileName}' успешно загружен.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Ошибка при загрузке файла: {response.ReasonPhrase}");
                    Console.WriteLine($"Дополнительная информация: {errorContent}");
                }
            }
        }

        public async Task<byte[]> DownloadFileAsync(string fileName)
        {
            var downloadUrl = "https://content.dropboxapi.com/2/files/download";
            var filePath = $"/Documents/{fileName}";

            // Удаляем старый заголовок, если был
            if (_httpClient.DefaultRequestHeaders.Contains("Dropbox-API-Arg"))
            {
                _httpClient.DefaultRequestHeaders.Remove("Dropbox-API-Arg");
            }

            _httpClient.DefaultRequestHeaders.Add(
                "Dropbox-API-Arg",
                Newtonsoft.Json.JsonConvert.SerializeObject(new { path = filePath })
            );

            var response = await _httpClient.PostAsync(downloadUrl, null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                Console.WriteLine($"❌ Ошибка при загрузке файла: {response.ReasonPhrase}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Дополнительная информация: {errorContent}");
                return null;
            }
        }
    }
}
