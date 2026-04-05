using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Entities;
using static System.Reflection.Metadata.BlobBuilder;

namespace Zalohovac_Editor_Strakos.Data
{
    public class ConfigRepository
    {
        private readonly string _filePath;

        public ConfigRepository(string filePath = "config.json")
        {
            _filePath = filePath;
        }

        public List<BackupJob> Load()
        {
            if (!File.Exists(_filePath))
                return new List<BackupJob>();

            string json = File.ReadAllText(_filePath);

            return JsonSerializer.Deserialize<List<BackupJob>>(json)
                ?? new List<BackupJob>();
        }
        public void Save(List<BackupJob> jobs)
        {
            string json = JsonSerializer.Serialize(jobs, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }
    }
}
