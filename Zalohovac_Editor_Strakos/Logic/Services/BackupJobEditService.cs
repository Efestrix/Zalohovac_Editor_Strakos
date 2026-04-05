using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Entities;
using static System.Reflection.Metadata.BlobBuilder;

namespace Zalohovac_Editor_Strakos.Logic.Services
{
    public class BackupJobEditService
    {
        public void ApplyDetailEdit(BackupJob job, int detailIndex, string value)
        {
            switch (detailIndex)
            {
                case 0:
                    if (Enum.TryParse<BackupMethod>(value, true, out BackupMethod method))
                        job.Method = method;
                    break;

                case 1:
                    job.Timing = value;
                    break;

                case 2:
                    if (int.TryParse(value, out int count))
                    {
                        job.Retention ??= new BackupRetention();
                        job.Retention.Count = count;
                    }
                    break;

                case 3:
                    if (int.TryParse(value, out int size))
                    {
                        job.Retention ??= new BackupRetention();
                        job.Retention.Size = size;
                    }
                    break;

                case 4:
                    job.Sources = value
                        .Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
                    break;

                case 5:
                    job.Targets = value
                        .Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
                    break;
            }
        }
    }
}
