using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Entities;

namespace Zalohovac_Editor_Strakos.Api
{
    public class ApiConfigRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiConfigRepository(string baseUrl, string token)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl.TrimEnd('/');

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<BackupJob>> LoadAsync()
        {
            List<BackupJob>? jobs =
                await _httpClient.GetFromJsonAsync<List<BackupJob>>($"{_baseUrl}/api/Jobs");

            return jobs ?? new List<BackupJob>();
        }

        public async Task CreateAsync(BackupJob job)
        {
            await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Jobs", job);
        }

        public async Task UpdateAsync(BackupJob job)
        {
            await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/Jobs/{job.Id}", job);
        }

        public async Task DeleteAsync(int id)
        {
            await _httpClient.DeleteAsync($"{_baseUrl}/api/Jobs/{id}");
        }
    }
}
