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
        private BackupJob _backupJob;

        private TextBox _method;
        private TextBox _timing;
        private TextBox _retention;
        private TextBox _source;
        private TextBox _targets;

        public BackupJobWindow(string title, Application application, IWindow? returnWindow, List<IComponent> components, int selectedIndex) 
            : base("Přehled záloh", application, returnWindow, components, selectedIndex)
        {
            _method = new TextBox("Metoda: \n", 32);
            _timing = new TextBox("Časování: \n", 16);
            _retention = new TextBox("Retence: \n", 64);
            _source = new TextBox("Zdroj: \n", 64);
            _targets = new TextBox("Cíle: \n", 64);

        }
    }
}
