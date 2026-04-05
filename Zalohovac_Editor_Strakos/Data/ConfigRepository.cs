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
        /*
        private List<BackupJob> LoadFromJson()
        {
            if (!File.Exists("config.json"))
                return new List<BackupJob>();

            string json = File.ReadAllText("config.json");

            return JsonSerializer.Deserialize<List<BackupJob>>(json)
                ?? new List<BackupJob>();
        }
        public void Save()
        {
            string json = JsonSerializer.Serialize(_jobs, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText("config.json", json);
        }
        */
    }
}
