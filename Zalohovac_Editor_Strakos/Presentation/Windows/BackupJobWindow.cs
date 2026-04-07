using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Entities;
using Zalohovac_Editor_Strakos.Presentation.Components;
using Zalohovac_Editor_Strakos.Data;

namespace Zalohovac_Editor_Strakos.Presentation.Windows
{
    public class BackupJobWindow : BaseWindow
    {
        private BackupJob _backupJob;

        private TextBox _methodTextBox;
        private TextBox _timingTextBox;
        private TextBox _retentionTextBox;
        private TextBox _sourceTextBox;
        private TextBox _targetsTextBox;

        private Label _methodLabel;
        private Label _timingLabel;
        private Label _retentionLabel;
        private Label _sourceLabel;
        private Label _targetsLabel;

        private Button _saveButton;

        private ConfigRepository _repository;
        public BackupJobWindow(Application application, BackupJob job, MainMenuWindow mainMenu) 
            : base("Přehled záloh", application, mainMenu)
        {
            _backupJob = job;
            _repository = new ConfigRepository();

            InitComponents();

            /*List<BackupJob> jobs = LoadFromJson();
            _backupJob = jobs.FirstOrDefault() ?? new BackupJob();*/

            SetComponentValues();
        }
        private void InitComponents()
        {

            _methodLabel = new Label("Metoda: \n");
            _timingLabel = new Label("Časování: \n");
            _retentionLabel = new Label("Retence: \n");
            _sourceLabel = new Label("Zdroj: \n");
            _targetsLabel = new Label("Cíle: \n");

            _methodTextBox = new TextBox("", 32);
            _timingTextBox = new TextBox("", 16);
            _retentionTextBox = new TextBox("", 64);
            _sourceTextBox = new TextBox("", 64);
            _targetsTextBox = new TextBox("", 64);

            _saveButton = new Button("Save", true);

            RegisterComponent(_methodLabel);
            RegisterComponent(_methodTextBox);

            RegisterComponent(_timingLabel);
            RegisterComponent(_timingTextBox);

            RegisterComponent(_retentionLabel);
            RegisterComponent(_retentionTextBox);

            RegisterComponent(_sourceLabel);
            RegisterComponent(_sourceTextBox);

            RegisterComponent(_targetsLabel);
            RegisterComponent(_targetsTextBox);

            RegisterComponent(_saveButton);

            _saveButton.Clicked += SaveButtonClicked;
        }
        private void SetComponentValues()
        {
            _methodTextBox.Value = _backupJob.Method.ToString();
            _timingTextBox.Value = _backupJob.Timing ?? "";
            _retentionTextBox.Value = (_backupJob.Retention?.Count ?? 0).ToString();
            _sourceTextBox.Value = string.Join(", ", _backupJob.Sources ?? new List<string>());
            _targetsTextBox.Value = string.Join(", ", _backupJob.Targets ?? new List<string>());
        }

        private void SaveButtonClicked()
        {
            _backupJob.Sources = _sourceTextBox.Value
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            _backupJob.Targets = _targetsTextBox.Value
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            if (!IsValidCron(_timingTextBox.Value))
            {
                throw new Exception("Neplatný CRON!");
            }
            _backupJob.Timing = _timingTextBox.Value;

            if (Enum.TryParse(_methodTextBox.Value, true, out BackupMethod method))
            {
                _backupJob.Method = method;
            }
            else
            {
                _backupJob.Method = BackupMethod.Full;
            }

            int count = 0;
            int.TryParse(_retentionTextBox.Value, out count);

            _backupJob.Retention = new BackupRetention
            {
                Count = count,
                Size = 0
            };

            List<BackupJob> jobs = _repository.Load();

            int existingIndex = jobs.FindIndex(j => j.Name == _backupJob.Name);

            if (existingIndex >= 0)
                jobs[existingIndex] = _backupJob;
            else
                jobs.Add(_backupJob);

            _repository.Save(jobs);
            Submit();
        }
        public bool IsValidCron(string cron)
        {
            return System.Text.RegularExpressions.Regex.IsMatch
                (
                    cron,
                    @"^(\*|\d+)(\s+(\*|\d+)){4}$"
                );
        }
    }
}
