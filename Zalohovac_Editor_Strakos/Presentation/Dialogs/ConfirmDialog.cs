using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Presentation.Components;
using Zalohovac_Editor_Strakos.Presentation.Windows;

namespace Zalohovac_Editor_Strakos.Presentation.Dialogs
{
    public class ConfirmDialog
    {
        public bool Visible { get; set; }
        public bool Result { get; private set; }

        private int _selected = 0; // 0 = OK, 1 = Cancel

        public void Render()
        {
            int w = 40;
            int h = 7;

            int left = (Console.WindowWidth - w) / 2;
            int top = (Console.WindowHeight - h) / 2;

            DrawBox(left, top, w, h);

            Console.SetCursorPosition(left + 2, top + 2);
            Console.Write("Opravdu smazat?");

            Console.SetCursorPosition(left + 8, top + 4);
            DrawButton("OK", _selected == 0);

            Console.SetCursorPosition(left + 20, top + 4);
            DrawButton("Cancel", _selected == 1);
        }

        public void HandleKey(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Tab)
            {
                _selected = (_selected + 1) % 2;
                return;
            }

            if (key.Key == ConsoleKey.Escape)
            {
                Result = false;
                Visible = false;
                return;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Result = _selected == 0;
                Visible = false;
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
            Result = false;
            Visible = false;
            _selected = 0;
        }
    }
}
