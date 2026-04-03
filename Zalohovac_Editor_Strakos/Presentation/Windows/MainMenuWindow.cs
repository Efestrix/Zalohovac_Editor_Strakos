using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Entities;
using Zalohovac_Editor_Strakos.Presentation.Components;
using Zalohovac_Editor_Strakos.Presentation.Dialogs;
using Zalohovac_Editor_Strakos.Presentation.ViewModels;

namespace Zalohovac_Editor_Strakos.Presentation.Windows
{
    public class MainMenuWindow : BaseWindow
    {
        private List<BackupJob> _jobs;

        private Table<JobListItem> _jobsTable;
        private Table<JobDetailItem> _detailTable;

        private Button _addButton;
        private Button _delButton;

        private int _activePanel = 0; // 0 = left, 1 = right
        public MainMenuWindow(Application application, IWindow? returnWindow = null)
            : base("Main Menu", application)
        {
            _jobs = LoadFromJson();

            _jobsTable = new Table<JobListItem>();
            _detailTable = new Table<JobDetailItem>();

            _addButton = new Button("Add");
            _delButton = new Button("Delete");

            RegisterComponent(_jobsTable);
            RegisterComponent(_detailTable);
            RegisterComponent(_addButton);
            RegisterComponent(_delButton);

            _jobsTable.ItemSelected += OpenSelectedJob;
            _addButton.Clicked += AddJob;
            _jobsTable.ItemSelected += UpdateDetailTable;
            _delButton.Clicked += DeleteSelectedJob;
            Closed += WindowClosed;

            RefreshTable();
        }

        private void RefreshTable()
        {
            Console.WriteLine($"Jobs count: {_jobs.Count}");

            _jobsTable.Items = _jobs
                .Select((job, index) => new JobListItem
                {
                    Name = job.Name
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
                new JobDetailItem { Property = "Retence", Value = job.Retention?.Count.ToString() ?? "0" },
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

            int totalWidth = Console.WindowWidth;

            List<string> left = _jobsTable.GetLines(true);
            List<string> right = _detailTable.GetLines(false);

            int leftWidth = totalWidth / 2 - 1;
            int rightWidth = totalWidth / 2 - 1;

            _jobsTable.StretchToWidth(leftWidth);
            _detailTable.StretchToWidth(rightWidth);

            int maxLines = Math.Max(left.Count, right.Count);

            for (int i = 0; i < maxLines; i++)
            {
                string l = i < left.Count ? left[i] : new string(' ', leftWidth);
                string r = i < right.Count ? right[i] : "";

                Console.WriteLine(l + r);
            }

            Console.WriteLine();
            _addButton.Render(false);
        }
        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                _activePanel = 1;
                return;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                _activePanel = 0;
                return;
            }

            if (_activePanel == 0)
                HandleLeftPanel(keyInfo);
            else
                HandleRightPanel(keyInfo);
        }
        public void HandleLeftPanel(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.UpArrow)
                _jobsTable.MoveUp();

            else if (keyInfo.Key == ConsoleKey.DownArrow)
                _jobsTable.MoveDown();

            else if (keyInfo.Key == ConsoleKey.Enter)
                _activePanel = 1;

            else if (keyInfo.Key == ConsoleKey.Delete)
                ShowDeleteDialog();

            else if (keyInfo.Key == ConsoleKey.A)
                ShowAddDialog();
        }
        public void HandleRightPanel(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.UpArrow)
                _jobsTable.MoveUp();

            else if (keyInfo.Key == ConsoleKey.DownArrow)
                _jobsTable.MoveDown();

            else if (keyInfo.Key == ConsoleKey.Enter)
                _activePanel = 0;
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

            BackupJobWindow window = new BackupJobWindow(_application, job, this);

            window.Submitted += () =>
            {
                _jobs.Add(job);
                Save();
                RefreshTable();
            };

            window.Show();
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
        private void DeleteSelectedJob()
        {
            JobListItem? selected = _jobsTable.SelectedItem;

            if (selected == null)
                return;

            int index = _jobsTable.Items.IndexOf(selected);

            _jobs.RemoveAt(index);

            Save();
            RefreshTable();

        }
        private void ShowAddDialog()
        {
            InputDialog dialog = new InputDialog("Nová konfigurace", _application);

            dialog.Submitted += () =>
            {
                BackupJob job = new BackupJob
                {
                    Name = string.IsNullOrWhiteSpace(dialog.Result)
                        ? $"Job {_jobs.Count + 1}"
                        : dialog.Result
                };

                _jobs.Add(job);
                Save();
                RefreshTable();
            };

            dialog.Show();
        }
        private void ShowDeleteDialog()
        {
            ConfirmDialog dialog = new ConfirmDialog("Smazat konfiguraci?", _application);

            dialog.Submitted += () =>
            {
                if (!dialog.Confirmed)
                    return;

                JobListItem? selected = _jobsTable.SelectedItem;
                if (selected == null)
                    return;

                int index = _jobsTable.Items.IndexOf(selected);

                _jobs.RemoveAt(index);
                Save();
                RefreshTable();
            };

            dialog.Show();
        }
        private void WindowClosed()
        {
            _application.Stop();
        }
    }
}
