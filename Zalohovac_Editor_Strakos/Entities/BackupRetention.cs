using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Zalohovac_Editor_Strakos.Entities
{
    public class BackupRetention
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("size")]
        public int Size { get; set; }
    }
}
