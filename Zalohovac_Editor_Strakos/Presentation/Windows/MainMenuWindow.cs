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

        private ConfirmDialog _confirmDialog = new ConfirmDialog();
        private InputDialog _inputDialog = new InputDialog();
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

            _jobsTable.ItemSelected += () =>
            {
                _jobsTable.Active = false;
                _detailTable.Active = true;
                _selectedIndex = 1;
            };

            Closed += WindowClosed;

            RefreshTable();

            _jobsTable.Active = true;
            _detailTable.Active = false;

            _addButton.Clicked += () => _inputDialog.Visible = true;
            _delButton.Clicked += () => _confirmDialog.Visible = true;
        }

        private void RefreshTable()
        {
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
            int totalHeight = Console.WindowHeight - 5;

            int leftWidth = totalWidth / 2 - 1;
            int rightWidth = totalWidth / 2 - 1;

            _jobsTable.StretchToWidth(leftWidth);
            _detailTable.StretchToWidth(rightWidth);

            List<string> left = _jobsTable.GetLines(_jobsTable.Active);
            List<string> right = _detailTable.GetLines(_detailTable.Active);

            int maxLines = Math.Max(left.Count, right.Count);
            maxLines = Math.Min(maxLines, totalHeight);

            for (int i = 0; i < maxLines; i++)
            {
                string l = i < left.Count
                    ? left[i].PadRight(leftWidth)
                    : new string(' ', leftWidth);

                string r = i < right.Count
                    ? right[i]
                    : "";

                bool leftRowSelected = _jobsTable.Active && i == _jobsTable.SelectedIndex + 1;
                bool rightRowSelected = _detailTable.Active && i == _detailTable.SelectedIndex + 1;

                // levá půlka
                if (leftRowSelected)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.Write(l.PadRight(leftWidth));
                Console.ResetColor();

                // pravá půlka
                if (rightRowSelected)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine(r);
                Console.ResetColor();
            }

            Console.WriteLine();
            _addButton.Render(_selectedIndex == 2);
            _delButton.Render(_selectedIndex == 3);

            if (_confirmDialog.Visible)
                _confirmDialog.Render();

            if (_inputDialog.Visible)
                _inputDialog.Render();
        }
        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Tab)
            {
                base.HandleKey(keyInfo);

                _jobsTable.Active = (_selectedIndex == 0);
                _detailTable.Active = (_selectedIndex == 1);

                return;
            }

            if (_detailTable.Active && keyInfo.Key == ConsoleKey.Escape)
            {
                _detailTable.Active = false;
                _jobsTable.Active = true;
                _selectedIndex = 0;
                return;
            }

            base.HandleKey(keyInfo);

            _jobsTable.Active = (_selectedIndex == 0);
            _detailTable.Active = (_selectedIndex == 1);
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
        private void WindowClosed()
        {
            _application.Stop();
        }
    }
}
