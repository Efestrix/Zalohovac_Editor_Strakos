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
    public class InputDialog
    {
        public string Title { get; set; } = "";
        public bool Visible { get; set; }
        public string Result { get; private set; } = "";

        private string _text = "";
        private int _selected = 0; // 0 = input, 1 = OK, 2 = Cancel

        public void Render()
        {
            int w = 50;
            int h = 9;

            int left = (Console.WindowWidth - w) / 2;
            int top = (Console.WindowHeight - h) / 2;

            DrawBox(left, top, w, h);

            Console.SetCursorPosition(left + 2, top + 1);
            Console.Write(Title);

            Console.SetCursorPosition(left + 2, top + 3);

            if (_selected == 0)
                Console.BackgroundColor = ConsoleColor.DarkGray;

            Console.Write(_text.PadRight(w - 4));
            Console.ResetColor();

            Console.SetCursorPosition(left + 10, top + 6);
            DrawButton("OK", _selected == 1);

            Console.SetCursorPosition(left + 25, top + 6);
            DrawButton("Cancel", _selected == 2);
        }

        public void HandleKey(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Tab)
            {
                _selected = (_selected + 1) % 3;
                return;
            }

            if (key.Key == ConsoleKey.Escape)
            {
                Result = "";
                Visible = false;
                return;
            }

            if (_selected == 0)
            {
                if (key.Key == ConsoleKey.Backspace && _text.Length > 0)
                    _text = _text.Substring(0, _text.Length - 1);

                else if (!char.IsControl(key.KeyChar))
                    _text += key.KeyChar;

                else if (key.Key == ConsoleKey.Enter)
                    _selected = 1;
            }
            else if (_selected == 1)
            {
                if (key.Key == ConsoleKey.Enter)
                {
                    Result = string.IsNullOrWhiteSpace(_text) ? "" : _text;
                    Visible = false;
                }
            }
            else if (_selected == 2)
            {
                if (key.Key == ConsoleKey.Enter)
                {
                    Result = "";
                    Visible = false;
                }
            }
        }

        private void DrawButton(string text, bool selected)
        {
            if (selected)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Write($"[ {text} ]");
            Console.ResetColor();
        }

        private void DrawBox(int x, int y, int w, int h)
        {
            for (int i = 0; i < h; i++)
            {
                Console.SetCursorPosition(x, y + i);

                if (i == 0 || i == h - 1)
                    Console.Write("+" + new string('-', w - 2) + "+");
                else
                    Console.Write("|" + new string(' ', w - 2) + "|");
            }
        }
        public void Reset()
        {
            _text = "";
            Result = "";
            _selected = 0;
            Visible = false;
        }
        public void SetInitialValue(string value)
        {
            _text = value ?? "";
            Result = "";
            _selected = 0;
        }
    }
}
