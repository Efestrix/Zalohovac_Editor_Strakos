using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Entities;
using Zalohovac_Editor_Strakos.Presentation.Components;
using Zalohovac_Editor_Strakos.Presentation.ViewModels;

namespace Zalohovac_Editor_Strakos.Presentation.Windows
{
    public class MainMenuWindow : BaseWindow
    {
        private List<BackupJob> _jobs;

        private Table<JobListItem> _jobsTable;
        private Table<JobDetailItem> _detailTable;

        private Button _addButton;
        public MainMenuWindow(Application application, IWindow? returnWindow = null)
            : base("Main Menu", application)
        {
            _jobs = LoadFromJson();

            _jobsTable = new Table<JobListItem>();
            _detailTable = new Table<JobDetailItem>();

            _addButton = new Button("Add");

            RegisterComponent(_jobsTable);
            RegisterComponent(_detailTable);
            RegisterComponent(_addButton);

            _jobsTable.ItemSelected += OpenSelectedJob;
            _addButton.Clicked += AddJob;
            _jobsTable.ItemSelected += UpdateDetailTable;

            RefreshTable();
        }

        private void RefreshTable()
        {
            Console.WriteLine($"Jobs count: {_jobs.Count}");

            _jobsTable.Items = _jobs
                .Select((job, index) => new JobListItem
                {
                    Name = $"Konfigurace_{index}",
                    Method = job.Method.ToString()
                })
                .ToList();

            UpdateDetailTable();
        }
        private void UpdateDetailTable()
        {
            JobListItem? selected = _jobsTable.SelectedItem;

            if (selected == null)
            {
                _detailTable.Items = new List<JobDetailItem>();
                return;
            }

            int index = _jobsTable.Items.IndexOf(selected);

            if (index < 0 || index >= _jobs.Count)
                return;

            BackupJob job = _jobs[index];
            _detailTable.Items = new List<JobDetailItem>
            {
                new JobDetailItem { Property = "Method", Value = job.Method.ToString() },
                new JobDetailItem { Property = "Časování", Value = job.Timing ?? "" },
                new JobDetailItem { Property = "Retence", Value = job.Retention.Count.ToString() ?? "" },
                new JobDetailItem { Property = "Zdroje", Value = string.Join(", ", job.Sources  ?? new List<string>()) },
                new JobDetailItem { Property = "Cíle", Value = string.Join(", ", job.Targets ?? new List<string>()) }
            };
        }
        public override void Show()
        {
            base.Show();
            _jobs = LoadFromJson();
            RefreshTable();
        }
        public override void Render()
        {
            Console.Clear();
            Console.WriteLine("=== Main Menu ===\n");

            UpdateDetailTable();

            List<string> left = _jobsTable.GetLines(true);
            List<string> right = _detailTable.GetLines(false);

            int totalWidth = Console.WindowWidth;
            int leftWidth = totalWidth / 2 - 2;

            int maxLines = Math.Max(left.Count, right.Count);

            for (int i = 0; i < maxLines; i++)
            {
                string l = i < left.Count ? left[i] : "";
                string r = i < right.Count ? right[i] : "";

                Console.WriteLine(l.PadRight(leftWidth) + " | " + r);
            }

            Console.WriteLine();
            _addButton.Render(false);
        }
        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            base.HandleKey(keyInfo);
        }

        private void OpenSelectedJob()
        {
            JobListItem? selected = _jobsTable.SelectedItem;

            if (selected == null)
                return;

            int index = _jobsTable.Items.IndexOf(selected);
            BackupJob job = _jobs[index];

            new BackupJobWindow(_application, job, this).Show();
        }
        private void AddJob()
        {
            BackupJob job = new BackupJob();
            _jobs.Add(job);

            new BackupJobWindow(_application, job, this).Show();
        }
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

    }
}
