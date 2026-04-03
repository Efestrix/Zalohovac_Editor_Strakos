using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Presentation.Components;
using Zalohovac_Editor_Strakos.Presentation.Windows;

namespace Zalohovac_Editor_Strakos.Presentation.Dialogs
{
    public class ConfirmDialog : BaseWindow
    {
        private Button _okButton;
        private Button _cancelButton;
        public bool Confirmed { get; private set; }

        public ConfirmDialog(string title, Application application) 
            : base(title, application)
        {
            _okButton = new Button("OK");
            _cancelButton = new Button("Cancel");

            RegisterComponent(_okButton);
            RegisterComponent(_cancelButton);

            _okButton.Clicked += () =>
            {
                Confirmed = true;
                Submit();
            };

            _cancelButton.Clicked += () =>
            {
                Confirmed = false;
                Close();
            };
        }
    }
}
