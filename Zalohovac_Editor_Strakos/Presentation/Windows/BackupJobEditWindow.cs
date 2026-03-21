using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zalohovac_Editor_Strakos.Presentation.Windows
{
    public class BackupJobEditWindow : BaseWindow
    {
        string _source = "";
        string _target = "";
        string _timing = "";
        string _method = "";
        string _retentionCount = "";
        string _retentionSize = "";

        public BackupJobEditWindow(string title, IWindow? returnWindow, int selectedIndex) : base(title, returnWindow, selectedIndex)
        {
        }
    }
}
