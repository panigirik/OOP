using System.Net.Http.Headers;

namespace ConsoleWord.Application.Dropbox
{
    public class DropboxStorageProvider : ICloudDropBoxStorageProvider
    {
        private const string AccessToken = "sl.u.AFrpbvVqJyhpL_ccAK0PPkv0wKJAE79zmCQS1FKE2r2nCx9Zfu7DUhSeq0jB21AJV3oWRJnvz1fVaZN4Hx-Kri23PftVkJbVM2sqKLIe0YigYvTozj_rSpBehQgq0c4lKlHNJA8yKtdTsyXX09NZGcFdT5GwUJmNFWdFKUb2K3gSIrijusop3kkWytzIxAQWC_7OOZoqL5erACTU1N6Y3Qf7ia3TvSJbt3SA5tsfZ_QVYdemmlwUmtCG5NXPmC9Rp2KIox28bx_d0cfQoqb4cnIUTpM2Bhd82QPg_2Q4whctoSbZnq-DA_HqOXeyPlMYIOi7v2MG4u98gKgKNzgxLYbO6zGpHtoUUDecWCZswX3ApuG_a5FkTSbALGXx0LzsD1kbgmDWtlE5SKs5k9nu4Btmc7U3zBXzH6sS3Jb0UTZwSsWKBVyEqhpNQURGc7RSOXtHp6JQj_5hAdH39Dk8rTcutohpg_EBLRvfNK0zCFY8gIpYQbbIKoSKRXs-8FeENTlcHQ4W5hXPUZpr03dH0ybJX3jQVW9jNd40YcGnuiihNT141_jXRZvjUmMABJiPEo3uiZSMDxNCIGS1-n4RjOejvbVt-C6bvSrUKyJx8gbFdzOt6tHA0WXBFwcGv5bhTq8YY-LI0R_9y8xT1lsXiWbVgL6wyGzybBcCjRcWmW0BdbhkMpaxPoNhV4NTKwJyhI2emy-yXAan6EwqzLlaETFhGBOsZvrpMTXsAD-6p_Q80itqudeMHHjfpr6V7J8z8a_XTY3xowvlyVKCrKzW__g-DOU2FL5IJebtontK9HO0WeaypLdlocDscoWRuN4L8FqI6m9nNeNd8gfRDIDRn6yqd4nI6z9ZIcPNsbUiL90pbk-_S19MKgU5MhnpcCnfGfbSu_boT5Xs_oy2FLCO6HHibeNgXutAnTNKlj8TVoHWlwdzqUdl5rSlRTeAqLZYutVvQeX7GUL4HWR2H-zX9RY4QjdoDm7cFqZXvT6tXCCDvhw9t6EejyPC7P6z3nEG-Y0F0qBzIoNBLNnoBcg-yxEJW0737QXAT8AOpuOqi_k1oI120wfbZ5AhwxjmPD2FgxLYdmIeASBBqbHibPyRvc87PMsiURsp0p4r88QnAq_oHDmoAQaZEmiY-n7hUPTEcDC7aylUPTOUeHP7sM-v_eZfWakwuFALke-8yQSU_FHVLT1ICOSPT-NiGo4hZp0Mxm78JuKRVQKJmq7tXEp_RnlRWaR8dkGnNli-FdqaP9QCGHH9Sy4adi-YadAVz_s2q85sE6Vxx0mIkU8iknIjDXsCXDYNxbC8CK41d2y2umqnOKI3Moa_HzwHbjNtTy9jI6onDrNHWDzA_0JuNf7_nu1O7JF9iat5WMLECAuusfSA_D1OeB3n7If7ZG6LB6XsNYnwGsqeUFrSe5CktA8KMVkARvUNwLyIEAPFH4hCxnUCIA" +
                                           "";
        private readonly HttpClient _httpClient;

        public DropboxStorageProvider()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }

        public async Task UploadFileAsync(string fileName, byte[] fileContent)
        {
            var uploadUrl = "https://content.dropboxapi.com/2/files/upload";
            var folderPath = "/Documents"; 

            using (var content = new ByteArrayContent(fileContent))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var dropboxArg = new
                {
                    path = $"{folderPath}/{fileName}", 
                    mode = "overwrite",
                    autorename = true,
                    mute = false
                };
                
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
                    Console.WriteLine($" Файл '{fileName}' успешно загружен.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($" Ошибка при загрузке файла: {response.ReasonPhrase}");
                    Console.WriteLine($"Дополнительная информация: {errorContent}");
                }
            }
        }

        public async Task<byte[]> DownloadFileAsync(string fileName)
        {
            var downloadUrl = "https://content.dropboxapi.com/2/files/download";
            var filePath = $"/Documents/{fileName}";
            
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
                Console.WriteLine($" Ошибка при загрузке файла: {response.ReasonPhrase}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Дополнительная информация: {errorContent}");
                return null;
            }
        }
    }
}
