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
            UpdateDetailTable();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            int headerHeight = 3;
            int leftX = 2;
            int panelTop = headerHeight + 1;
            int panelHeight = height - headerHeight - 3;

            int dividerX = width / 2;
            int leftPanelWidth = dividerX - leftX - 1;
            int rightX = dividerX + 1;
            int rightPanelWidth = width - rightX - 2;

            Console.SetCursorPosition(0, 0);

            DrawBackground(width, height);
            DrawHeader(width);
            DrawPanels(leftX, panelTop, leftPanelWidth, rightX, rightPanelWidth, panelHeight);
            DrawJobs(leftX + 2, panelTop + 2, leftPanelWidth - 4);
            DrawDetails(rightX + 2, panelTop + 2, rightPanelWidth - 4);
            DrawButtons(rightX + 4, panelTop + panelHeight - 3);

            if (_confirmDialog.Visible)
                _confirmDialog.Render();

            if (_inputDialog.Visible)
                _inputDialog.Render();
        }
        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (_confirmDialog.Visible)
            {
                _confirmDialog.HandleKey(keyInfo);
                return;
            }

            if (_inputDialog.Visible)
            {
                _inputDialog.HandleKey(keyInfo);
                return;
            }

            if (keyInfo.Key == ConsoleKey.Tab)
            {
                base.HandleKey(keyInfo);

                _jobsTable.Active = _selectedIndex == 0;
                _detailTable.Active = _selectedIndex == 1;

                return;
            }

            if (_detailTable.Active && keyInfo.Key == ConsoleKey.Escape)
            {
                _detailTable.Active = false;
                _jobsTable.Active = true;
                _selectedIndex = 0;
                return;
            }

            if (_jobsTable.Active)
            {
                _jobsTable.HandleKey(keyInfo);
            }
            else if (_detailTable.Active)
            {
                _detailTable.HandleKey(keyInfo);
            }
            else
            {
                base.HandleKey(keyInfo);
            }

            _jobsTable.Active = _selectedIndex == 0;
            _detailTable.Active = _selectedIndex == 1;
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
        private void DrawBackground(int width, int height)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write(new string(' ', width));
            }

            Console.ResetColor();
        }

        private void DrawHeader(int width)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.SetCursorPosition(0, 0);
            Console.Write((" Přehled záloh ").PadRight(width));

            Console.SetCursorPosition(0, 1);
            Console.Write(new string(' ', width));

            Console.ResetColor();
        }

        private void DrawPanels(int leftX, int top, int leftWidth, int rightX, int rightWidth, int height)
        {
            DrawBox(leftX, top, leftWidth, height);
            DrawBox(rightX, top, rightWidth, height);
        }

        private void DrawBox(int x, int y, int width, int height)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Blue;

            Console.SetCursorPosition(x, y);
            Console.Write("┌" + new string('─', width - 2) + "┐");

            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│" + new string(' ', width - 2) + "│");
            }

            Console.SetCursorPosition(x, y + height - 1);
            Console.Write("└" + new string('─', width - 2) + "┘");

            Console.ResetColor();
        }

        private void DrawJobs(int x, int y, int width)
        {
            for (int i = 0; i < _jobs.Count; i++)
            {
                Console.SetCursorPosition(x, y + i * 2);

                bool selected = _jobsTable.Active && i == _jobsTable.SelectedIndex;

                if (selected)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                string text = _jobs[i].Name ?? $"Konfigurace {i + 1}";
                if (text.Length > width)
                    text = text.Substring(0, width - 3) + "...";

                Console.Write(text.PadRight(width));
                Console.ResetColor();
            }
        }

        private void DrawDetails(int x, int y, int width)
        {
            if (_jobs.Count == 0 || _jobsTable.SelectedItem == null)
                return;

            int index = _jobsTable.Items.IndexOf(_jobsTable.SelectedItem);
            if (index < 0 || index >= _jobs.Count)
                return;

            BackupJob job = _jobs[index];

            List<(string Label, string Value)> rows = new()
            {
                ("Metoda:", job.Method.ToString()),
                ("Časování:", job.Timing ?? ""),
                ("Retence:", $"{job.Retention?.Count ?? 0} záloh, velikost balíčku {job.Retention?.Size ?? 0}"),
                ("Zdroje:", string.Join(", ", job.Sources ?? new List<string>())),
                ("Cíle:", string.Join(", ", job.Targets ?? new List<string>()))
            };

            int row = 0;

            for (int i = 0; i < rows.Count; i++)
            {
                bool selected = _detailTable.Active && i == _detailTable.SelectedIndex;

                Console.SetCursorPosition(x, y + row);
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(rows[i].Label.PadRight(width));
                row++;

                Console.SetCursorPosition(x + 2, y + row);

                if (selected)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                string value = rows[i].Value ?? "";
                if (value.Length > width - 2)
                    value = value.Substring(0, width - 5) + "...";

                Console.Write(value.PadRight(width - 2));
                Console.ResetColor();

                row += 2;
            }
        }

        private void DrawButtons(int x, int y)
        {
            DrawButton(x, y, "OK", _selectedIndex == 2);
            DrawButton(x + 20, y, "Storno", _selectedIndex == 3);
        }

        private void DrawButton(int x, int y, string text, bool selected)
        {
            Console.SetCursorPosition(x, y);

            if (selected)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Write($"[ {text} ]");
            Console.ResetColor();
        }
    }
}
