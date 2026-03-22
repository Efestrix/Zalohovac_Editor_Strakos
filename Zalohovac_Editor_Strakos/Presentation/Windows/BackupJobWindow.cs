using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Entities;
using Zalohovac_Editor_Strakos.Presentation.Components;

namespace Zalohovac_Editor_Strakos.Presentation.Windows
{
    public class BackupJobWindow : BaseWindow
    {
        private TextBox _methodTextBox;
        private TextBox _timingTextBox;
        private TextBox _retentionTextBox;
        private TextBox _sourceTextBox;
        private TextBox _targetsTextBox;

        private Button _saveButton;

        public BackupJobWindow(Application application) 
            : base("Přehled záloh", application)
        {
            _methodTextBox = new TextBox("Metoda: \n", 32);
            _timingTextBox = new TextBox("Časování: \n", 16);
            _retentionTextBox = new TextBox("Retence: \n", 64);
            _sourceTextBox = new TextBox("Zdroj: \n", 64);
            _targetsTextBox = new TextBox("Cíle: \n", 64);

            _saveButton = new Button("Save", true);

            RegisterComponent(_methodTextBox);
            RegisterComponent(_timingTextBox);
            RegisterComponent(_retentionTextBox);
            RegisterComponent(_sourceTextBox);
            RegisterComponent(_targetsTextBox);
            RegisterComponent(_saveButton);
        }
    }
}
