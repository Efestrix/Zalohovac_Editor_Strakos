using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Presentation.Components;
using Zalohovac_Editor_Strakos.Presentation.Windows;

namespace Zalohovac_Editor_Strakos.Presentation.Dialogs
{
    public class InputDialog : BaseWindow
    {
        private TextBox _inputTextBox;
        private Button _okButton;
        private Button _cancelButton;

        public string Result { get; private set; }
        public InputDialog(string title, Application application) 
            : base(title, application)
        {
            _inputTextBox = new TextBox("", 30);
            _okButton = new Button("OK");
            _cancelButton = new Button("Cancel");

            RegisterComponent(_inputTextBox);
            RegisterComponent(_okButton);
            RegisterComponent(_cancelButton);

            _okButton.Clicked += () =>
            {
                Result = _inputTextBox.Value;
                Submit();
            };

            _cancelButton.Clicked += () =>
            {
                Close();
            };
        }
        public override void Render()
        {
            int width = 40;
            int height = 6;

            int left = (Console.WindowWidth - width) / 2;
            int top = (Console.WindowHeight - height) / 2;

            DrawBox(left, top, width, height);

            Console.SetCursorPosition(left + 2, top + 1);
            Console.WriteLine(_title);

            Console.SetCursorPosition(left + 2, top + 2);
            _inputTextBox.Render(true);

            Console.SetCursorPosition(left + 2, top + 4);
            _okButton.Render(false);

            Console.SetCursorPosition(left + 10, top + 4);
            _cancelButton.Render(false);
        }
        private void DrawBox(int x, int y, int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);

                if (i == 0 || i == height - 1)
                    Console.WriteLine("+" + new string('-', width - 2) + "+");
                else
                    Console.WriteLine("|" + new string(' ', width - 2) + "|");
            }
        }
    }
}
