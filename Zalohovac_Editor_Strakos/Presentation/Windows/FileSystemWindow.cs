using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Presentation.Components;

namespace Zalohovac_Editor_Strakos.Presentation.Windows
{
    public class FileSystemWindow : BaseWindow
    {
        public List<string> Result { get; private set; } = new List<string>();

        private string _currentPath;
        private List<string> _items = new List<string>();

        private int _leftSelectedIndex = 0;
        private int _rightSelectedIndex = 0;

        private int _leftOffset = 0;
        private int _rightOffset = 0;

        private bool _layoutDrawn = false;

        private int _focus = 0; // 0 = left, 1 = right, 2 = OK, 3 = Cancel

        private Button _okButton;
        private Button _cancelButton;

        public FileSystemWindow(Application app, List<string>? initialSelection = null, IWindow? returnWindow = null) 
            : base("Výběr složek", app, returnWindow)
        {
            _currentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Result = initialSelection != null
                ? new List<string>(initialSelection)
                : new List<string>();

            _okButton = new Button("OK", true);
            _cancelButton = new Button("Cancel", true);

            LoadItems();
        }

        private void LoadItems()
        {
            _items.Clear();
            _items.Add("..");

            try
            {
                List<string> dirs = Directory.GetDirectories(_currentPath)
                    .OrderBy(x => x)
                    .ToList();

                _items.AddRange(dirs);
            }
            catch
            {
            }

            if (_leftSelectedIndex >= _items.Count)
                _leftSelectedIndex = Math.Max(0, _items.Count - 1);
        }

        public override void Render()
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            int headerHeight = 2;
            int panelTop = headerHeight + 1;
            int panelHeight = height - headerHeight - 5;

            int leftX = 2;
            int dividerX = width / 2;
            int leftWidth = dividerX - leftX;
            int rightX = dividerX;
            int rightWidth = width - rightX - 1;

            if (!_layoutDrawn)
            {
                DrawBackground(width, height);
                DrawHeader(width);
                DrawBox(leftX, panelTop, leftWidth, panelHeight);
                DrawBox(rightX, panelTop, rightWidth, panelHeight);
                _layoutDrawn = true;
            }

            Console.SetCursorPosition(leftX + 2, panelTop + 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            string currentPathText = TruncateMiddle($"Aktuální cesta: {_currentPath}", leftWidth - 4);
            Console.Write(currentPathText.PadRight(leftWidth - 4));

            Console.SetCursorPosition(rightX + 2, panelTop + 1);
            Console.Write("Vybrané složky".PadRight(rightWidth - 4));
            Console.ResetColor();

            DrawLeftPanel(leftX + 2, panelTop + 3, leftWidth - 4, panelHeight - 5);
            DrawRightPanel(rightX + 2, panelTop + 3, rightWidth - 4, panelHeight - 5);

            Console.SetCursorPosition(rightX + 4, panelTop + panelHeight + 1);
            _okButton.Render(_focus == 2);

            Console.SetCursorPosition(rightX + 20, panelTop + panelHeight + 1);
            _cancelButton.Render(_focus == 3);

            Console.SetCursorPosition(2, height - 1);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[Tab] přepnout panel/tlačítka  [Enter] otevřít  [Space] přidat  [Delete] odebrat  [Esc] zpět".PadRight(width));
            Console.ResetColor();
        }

        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                Close();
                return;
            }
            else if (keyInfo.Key == ConsoleKey.Tab)
            {
                _focus = (_focus + 1) % 4;
                return;
            }

            switch (_focus)
            {
                case 0:
                    HandleLeftPanel(keyInfo);
                    break;

                case 1:
                    HandleRightPanel(keyInfo);
                    break;

                case 2:
                    if (keyInfo.Key == ConsoleKey.Enter)
                        Submit();
                    break;

                case 3:
                    if (keyInfo.Key == ConsoleKey.Enter)
                        Close();
                    break;
            }
        }

        private void HandleLeftPanel(ConsoleKeyInfo keyInfo)
        {
            int visibleHeight = Math.Max(1, Console.WindowHeight - 2 - 5 - 5);

            if (keyInfo.Key == ConsoleKey.UpArrow && _leftSelectedIndex > 0)
            {
                _leftSelectedIndex--;

                if (_leftSelectedIndex < _leftOffset)
                    _leftOffset = _leftSelectedIndex;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow && _leftSelectedIndex < _items.Count - 1)
            {
                _leftSelectedIndex++;

                if (_leftSelectedIndex >= _leftOffset + visibleHeight)
                    _leftOffset = _leftSelectedIndex - visibleHeight + 1;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (_items.Count == 0)
                    return;

                string selected = _items[_leftSelectedIndex];

                if (selected == "..")
                {
                    DirectoryInfo? parent = Directory.GetParent(_currentPath);
                    if (parent != null)
                    {
                        _currentPath = parent.FullName;
                        _leftSelectedIndex = 0;
                        _leftOffset = 0;
                        LoadItems();
                    }
                }
                else if (Directory.Exists(selected))
                {
                    _currentPath = selected;
                    _leftSelectedIndex = 0;
                    _leftOffset = 0;
                    LoadItems();
                }
            }
            else if (keyInfo.Key == ConsoleKey.Spacebar)
            {
                if (_items.Count == 0)
                    return;

                string selected = _items[_leftSelectedIndex];
                if (selected == "..")
                    return;

                if (!Result.Contains(selected))
                    Result.Add(selected);
            }
        }

        private void HandleRightPanel(ConsoleKeyInfo keyInfo)
        {
            int visibleHeight = Math.Max(1, Console.WindowHeight - 2 - 5 - 5);

            if (keyInfo.Key == ConsoleKey.UpArrow && _rightSelectedIndex > 0)
            {
                _rightSelectedIndex--;

                if (_rightSelectedIndex < _rightOffset)
                    _rightOffset = _rightSelectedIndex;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow && _rightSelectedIndex < Result.Count - 1)
            {
                _rightSelectedIndex++;

                if (_rightSelectedIndex >= _rightOffset + visibleHeight)
                    _rightOffset = _rightSelectedIndex - visibleHeight + 1;
            }
            else if (keyInfo.Key == ConsoleKey.Delete)
            {
                if (Result.Count == 0)
                    return;

                Result.RemoveAt(_rightSelectedIndex);

                if (_rightSelectedIndex >= Result.Count)
                    _rightSelectedIndex = Math.Max(0, Result.Count - 1);

                if (_rightSelectedIndex < _rightOffset)
                    _rightOffset = _rightSelectedIndex;
            }
        }

        private void DrawLeftPanel(int x, int y, int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                int itemIndex = _leftOffset + i;

                Console.SetCursorPosition(x, y + i);

                if (itemIndex >= _items.Count)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(new string(' ', width));
                    Console.ResetColor();
                    continue;
                }

                bool selected = _focus == 0 && itemIndex == _leftSelectedIndex;

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

                string item = _items[itemIndex] == ".." ? ".." : Path.GetFileName(_items[i]);
                if (string.IsNullOrWhiteSpace(item))
                    item = _items[itemIndex];

                item = TruncateMiddle(item, width);

                Console.Write(item.PadRight(width));
                Console.ResetColor();
            }
        }

        private void DrawRightPanel(int x, int y, int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                int itemIndex = _rightOffset + i;

                Console.SetCursorPosition(x, y + i);

                if (itemIndex >= Result.Count)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(new string(' ', width));
                    Console.ResetColor();
                    continue;
                }

                bool selected = _focus == 1 && itemIndex == _rightSelectedIndex;

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

                string item = TruncateMiddle(Result[itemIndex], width);

                Console.Write(item.PadRight(width));
                Console.ResetColor();
            }
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
            Console.Write((" Výběr složek ").PadRight(width));

            Console.SetCursorPosition(0, 1);
            Console.Write(new string(' ', width));

            Console.ResetColor();
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
        private string TruncateMiddle(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            if (maxLength <= 3)
                return text.Substring(0, maxLength);

            int firstPart = (maxLength - 3) / 2;
            int secondPart = maxLength - 3 - firstPart;

            return text.Substring(0, firstPart) + "..." + text.Substring(text.Length - secondPart);
        }
    }
}
