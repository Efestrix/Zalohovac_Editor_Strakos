using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Entities;
using static System.Reflection.Metadata.BlobBuilder;

namespace Zalohovac_Editor_Strakos.Presentation.Renderers
{
    public class MainMenuRenderer
    {
        /*
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
                ("Retence:", $"{job.Retention?.Count ?? 0} záloh, velikost balíčku {job.Retention?.Size ?? 0}"),
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
        */
    }
}
