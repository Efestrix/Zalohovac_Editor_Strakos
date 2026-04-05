using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Entities;
using Zalohovac_Editor_Strakos.Presentation.Components;
using Zalohovac_Editor_Strakos.Presentation.ViewModels;
using static System.Reflection.Metadata.BlobBuilder;

namespace Zalohovac_Editor_Strakos.Presentation.Renderers
{
    public class MainMenuRenderer
    {
        public void Render(
            List<BackupJob> jobs,
            Table<JobListItem> jobsTable,
            Table<JobDetailItem> detailTable)
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            int headerHeight = 2;
            int leftX = 2;
            int panelTop = headerHeight + 1;
            int panelHeight = height - headerHeight - 3;

            int dividerX = width / 2;
            int leftPanelWidth = dividerX - leftX;
            int rightX = dividerX;
            int rightPanelWidth = width - rightX - 1;

            DrawBackground(width, height);
            DrawHeader(width);
            DrawPanels(leftX, panelTop, leftPanelWidth, rightX, rightPanelWidth, panelHeight);

            DrawJobs(jobs, jobsTable, leftX + 2, panelTop + 2, leftPanelWidth - 4, panelHeight - 6);
            DrawDetails(jobs, jobsTable, detailTable, rightX + 2, panelTop + 2, rightPanelWidth - 4, panelHeight - 6);
        }

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

        private void DrawJobs(
            List<BackupJob> jobs,
            Table<JobListItem> jobsTable,
            int x, int y, int width, int height)
        {
            int maxVisible = Math.Max(1, height / 2);

            for (int i = 0; i < maxVisible; i++)
            {
                Console.SetCursorPosition(x, y + i * 2);

                if (i >= jobs.Count)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(new string(' ', width));
                    Console.ResetColor();
                    continue;
                }

                bool selected = jobsTable.Active && i == jobsTable.SelectedIndex;

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

                string text = jobs[i].Name ?? $"Konfigurace {i + 1}";
                if (text.Length > width)
                    text = text.Substring(0, width - 3) + "...";

                Console.Write((" " + text).PadRight(width));
                Console.ResetColor();
            }
        }

        private void DrawDetails(
            List<BackupJob> jobs,
            Table<JobListItem> jobsTable,
            Table<JobDetailItem> detailTable,
            int x, int y, int width, int height)
        {
            if (jobs.Count == 0 || jobsTable.SelectedItem == null)
                return;

            int index = jobsTable.Items.IndexOf(jobsTable.SelectedItem);
            if (index < 0 || index >= jobs.Count)
                return;

            BackupJob job = jobs[index];

            List<(string Label, string Value)> rows = new()
            {
                ("Metoda:", job.Method.ToString()),
                ("Časování:", job.Timing ?? ""),
                ("Retence Count:", (job.Retention?.Count ?? 0).ToString()),
                ("Retence Size:", (job.Retention?.Size ?? 0).ToString()),
                ("Zdroje:", string.Join(", ", job.Sources ?? new List<string>())),
                ("Cíle:", string.Join(", ", job.Targets ?? new List<string>()))
            };

            int row = 0;

            for (int i = 0; i < rows.Count; i++)
            {
                bool selected = detailTable.Active && i == detailTable.SelectedIndex;

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
    }
}
