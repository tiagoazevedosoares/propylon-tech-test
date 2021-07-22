using System;
using System.Net.Http;
using DTO.Users;
using Microsoft.AspNetCore.Http;
using Frontend.Extensions;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using DTO.Documents;

namespace Frontend.Services
{
    public class APIService : IAPIService
    {
        private readonly IHttpClientFactory _clientFactory;

        public APIService(IHttpClientFactory _clientFactory)
        {
            this._clientFactory = _clientFactory;
        }

        public async Task<DownloadModel> DownloadDocument(DownloadModel downloadModel, UserModelWithToken userModel)
        {
            HttpClient client = _clientFactory.CreateClient("documents");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userModel.Token);
            var content = new StringContent(JsonSerializer.Serialize(downloadModel), System.Text.Encoding.UTF8, "application/json");

            var result = await client.GetAsync($"/documents/download?documentName={downloadModel.DocumentName}&revision={downloadModel.Revision}");
            var json = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var download = JsonSerializer.Deserialize<DownloadModel>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                return download;
            }

            throw new Exception(json);
        }

        public async Task<IEnumerable<DocumentModel>> GetDocuments(UserModelWithToken userModel)
        {
            HttpClient client = _clientFactory.CreateClient("documents");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userModel.Token);

            var result = await client.GetAsync("/documents");
            var json = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var documents = JsonSerializer.Deserialize<IEnumerable<DocumentModel>>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                return documents;
            }

            throw new Exception(json);
        }

        public async Task<UserModelWithToken> Login(AuthenticateModel userModel)
        {
            HttpClient client = _clientFactory.CreateClient("documents");
            var content = new StringContent(JsonSerializer.Serialize(userModel), System.Text.Encoding.UTF8, "application/json");

            var result = await client.PostAsync("/users/authenticate", content);
            var json = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var user = JsonSerializer.Deserialize<UserModelWithToken>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                return user;
            }

            throw new Exception(json);
        }

        public async Task<int> UploadDocument(UploadModel uploadModel, UserModelWithToken userModel)
        {
            HttpClient client = _clientFactory.CreateClient("documents");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userModel.Token);
            var content = new StringContent(JsonSerializer.Serialize(uploadModel), System.Text.Encoding.UTF8, "application/json");

            var result = await client.PostAsync("/documents/upload", content);
            var json = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var id = int.Parse(json);

                return id;
            }

            throw new Exception(json);
        }

        public async Task<int> Register(RegisterModel registerModel)
        {
            HttpClient client = _clientFactory.CreateClient("documents");
            var content = new StringContent(JsonSerializer.Serialize(registerModel), System.Text.Encoding.UTF8, "application/json");

            var result = await client.PostAsync("/users/register", content);
            var json = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var id = int.Parse(json);

                return id;
            }

            throw new Exception(json);
        }
    }
}
