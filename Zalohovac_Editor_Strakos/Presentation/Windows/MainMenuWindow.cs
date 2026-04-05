using System.Linq.Expressions;
using System.Text.Json;
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

        private int _editingDetailIndex = -1;

        private ConfirmDialog _confirmDialog = new ConfirmDialog();
        private InputDialog _inputDialog = new InputDialog();
        public MainMenuWindow(Application application, IWindow? returnWindow = null)
            : base("Main Menu", application)
        {
            _jobs = LoadFromJson();

            _jobsTable = new Table<JobListItem>();
            _detailTable = new Table<JobDetailItem>();

            RegisterComponent(_jobsTable);
            RegisterComponent(_detailTable);

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
                new JobDetailItem { Property = "Retence Count", Value = (job.Retention?.Count ?? 0).ToString() },
                new JobDetailItem { Property = "Retence Size", Value = (job.Retention?.Size ?? 0).ToString() },
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

            int headerHeight = 2;
            int margin = 1;

            int leftX = 2;
            int panelTop = headerHeight + 1;
            int panelHeight = height - headerHeight - 3;

            int dividerX = width / 2;
            int leftPanelWidth = dividerX - leftX;
            int rightX = dividerX;
            int rightPanelWidth = width - rightX - 1;

            Console.SetCursorPosition(0, 0);

            DrawBackground(width, height);
            DrawHeader(width);
            DrawPanels(leftX, panelTop, leftPanelWidth, rightX, rightPanelWidth, panelHeight);
            DrawJobs(leftX + 2, panelTop + 2, leftPanelWidth - 4, panelHeight - 6);
            DrawDetails(rightX + 2, panelTop + 2, rightPanelWidth - 4, panelHeight - 6);

            DrawHotKeys(leftX + 2, panelTop + panelHeight - 2);

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

                if (!_confirmDialog.Visible)
                {
                    if (_confirmDialog.Result)
                        DeleteSelectedJob();

                    _confirmDialog.Reset();
                }

                return;
            }

            if (_inputDialog.Visible)
            {
                _inputDialog.HandleKey(keyInfo);
                
                if (!_inputDialog.Visible)
                {
                    if (!string.IsNullOrWhiteSpace(_inputDialog.Result))
                    {
                        if (_editingDetailIndex == -1)
                        {
                            BackupJob job = new BackupJob
                            {
                                Name = _inputDialog.Result,
                            };

                            _jobs.Add(job);
                        }
                        else
                        {
                            ApplyDetailEdit(_inputDialog.Result);
                        }

                        Save();
                        RefreshTable();
                    }
                    _editingDetailIndex = -1;
                    _inputDialog.Reset();
                }

                return;
            }

            if (_jobsTable.Active && (keyInfo.Key == ConsoleKey.A ||keyInfo.KeyChar == 'a'))
            {
                _inputDialog.Reset();
                _inputDialog.Title = "Zadej název konfigurace";
                _inputDialog.Visible = true;
                return;
            }

            if (_jobsTable.Active && (keyInfo.Key == ConsoleKey.Delete))
            {
                _confirmDialog.Visible = true;
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
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    _editingDetailIndex = _detailTable.SelectedIndex;
                    OpenDetailEditor();
                    return;
                }

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

        private void DrawJobs(int x, int y, int width, int height)
        {
            int maxVisible = Math.Max(1, height / 2);

            for (int i = 0; i < maxVisible; i++)
            {
                Console.SetCursorPosition(x, y + i * 2);

                if (i >= _jobs.Count)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(new string(' ', width));
                    Console.ResetColor();
                    continue;
                }

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

                Console.Write((" " + text).PadRight(width));
                Console.ResetColor();
            }
        }

        private void DrawDetails(int x, int y, int width, int height)
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
                ("Retence Count:", (job.Retention?.Count ?? 0).ToString()),
                ("Retence Size:", (job.Retention?.Size ?? 0).ToString()),
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
                Console.ResetColor();

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
        
        private void DrawHotKeys(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ResetColor();
        }
        private void DeleteSelectedJob()
        {
            JobListItem? selected = _jobsTable.SelectedItem;
            if (selected == null)
                return;

            int index = _jobsTable.Items.IndexOf(selected);
            if (index < 0 || index >= _jobs.Count)
                return;

            _jobs.RemoveAt(index);
            Save();
            RefreshTable();

            if (_jobs.Count == 0)
            {
                _jobsTable.Active = true;
                _detailTable.Active = false;
                _selectedIndex = 0;
            }
        }
        private void OpenDetailEditor()
        {
            if (_jobsTable.SelectedItem == null)
                return;

            int jobIndex = _jobsTable.Items.IndexOf(_jobsTable.SelectedItem);
            if (jobIndex < 0 || jobIndex >= _jobs.Count)
                return;

            BackupJob job = _jobs[jobIndex];

            _inputDialog.Reset();

            switch (_editingDetailIndex)
            {
                case 0:
                    _inputDialog.Title = "Zadej metodu (Full/Differential/Incremental)";
                    _inputDialog.SetInitialValue(job.Method.ToString());
                    _inputDialog.Visible = true;
                    return;

                case 1:
                    _inputDialog.Title = "Zadej časování (CRON)";
                    _inputDialog.SetInitialValue(job.Timing ?? "");
                    _inputDialog.Visible = true;
                    return;

                case 2:
                    _inputDialog.Title = "Zadej retention count:";
                    _inputDialog.SetInitialValue(job.Retention?.Count.ToString() ?? "0");
                    _inputDialog.Visible = true;
                    return;

                case 3:
                    _inputDialog.Title = "Zadej retention size:";
                    _inputDialog.SetInitialValue(job.Retention?.Size.ToString() ?? "0");
                    _inputDialog.Visible = true;
                    return;

                case 4:
                    FileSystemWindow windowSource = new FileSystemWindow(_application, job.Sources, this);
                    windowSource.Submitted += () =>
                    {
                        job.Sources = new List<string>(windowSource.Result);
                        Save();
                        RefreshTable();
                    };
                    windowSource.Show();
                    return;

                case 5:
                    FileSystemWindow windowTarget = new FileSystemWindow(_application, job.Targets, this);
                    windowTarget.Submitted += () =>
                    {
                        job.Targets = new List<string>(windowTarget.Result);
                        Save();
                        RefreshTable();
                    };
                    windowTarget.Show();
                    return;
            }

            _inputDialog.Visible = true;
        }
        private void ApplyDetailEdit(string value)
        {
            if (_jobsTable.SelectedItem == null)
                return;

            int jobIndex = _jobsTable.Items.IndexOf(_jobsTable.SelectedItem);
            if (jobIndex < 0 || jobIndex >= _jobs.Count)
                return;

            BackupJob job = _jobs[jobIndex];

            switch (_editingDetailIndex)
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
