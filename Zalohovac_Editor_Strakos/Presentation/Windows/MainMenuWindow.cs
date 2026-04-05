using System.Linq.Expressions;
using System.Text.Json;
using Zalohovac_Editor_Strakos.Data;
using Zalohovac_Editor_Strakos.Entities;
using Zalohovac_Editor_Strakos.Logic.Services;
using Zalohovac_Editor_Strakos.Presentation.Components;
using Zalohovac_Editor_Strakos.Presentation.Dialogs;
using Zalohovac_Editor_Strakos.Presentation.Renderers;
using Zalohovac_Editor_Strakos.Presentation.ViewModels;

namespace Zalohovac_Editor_Strakos.Presentation.Windows
{
    public class MainMenuWindow : BaseWindow
    {
        private List<BackupJob> _jobs;

        private Table<JobListItem> _jobsTable;
        private Table<JobDetailItem> _detailTable;

        private int _editingDetailIndex = -1;

        private bool _layoutDrawn = false;

        private ConfirmDialog _confirmDialog = new ConfirmDialog();
        private InputDialog _inputDialog = new InputDialog();

        private readonly ConfigRepository _repository;
        private readonly BackupJobEditService _editService;
        private readonly MainMenuRenderer _renderer;
        public MainMenuWindow(Application application, IWindow? returnWindow = null)
            : base("Main Menu", application)
        {
            _repository = new ConfigRepository();
            _editService = new BackupJobEditService();
            _renderer = new MainMenuRenderer();

            _jobs = _repository.Load();

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
            _jobs = _repository.Load();
            RefreshTable();
        }
        public override void Render()
        {
            UpdateDetailTable();
            _renderer.Render(_jobs, _jobsTable, _detailTable);

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

                        _repository.Save(_jobs);
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
        private void WindowClosed()
        {
            _application.Stop();
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
            _repository.Save(_jobs);
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
                        _repository.Save(_jobs);
                        RefreshTable();
                    };
                    windowSource.Show();
                    return;

                case 5:
                    FileSystemWindow windowTarget = new FileSystemWindow(_application, job.Targets, this);
                    windowTarget.Submitted += () =>
                    {
                        job.Targets = new List<string>(windowTarget.Result);
                        _repository.Save(_jobs);
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
