using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Helpers;

namespace Zalohovac_Editor_Strakos.Presentation.Components
{
    public class Table<T> : BaseComponent
        where T : class
    {
        public event Action? ItemSelected;

        public override bool Selectable => true;

        public bool Active { get; set; } = false;

        public T? SelectedItem
        {
            get
            {
                if (Items == null || Items.Count == 0)
                    return default;

                if (_selectedIndex < 0)
                    _selectedIndex = 0;

                if (_selectedIndex >= Items.Count)
                    _selectedIndex = Items.Count - 1;

                return Items[_selectedIndex];
            }
        }
        public int SelectedIndex => _selectedIndex;

        public List<T> Items { get; set; }

        private int _count;
        private int _offset;
        private int _selectedIndex;
        private List<string> _headers;
        private List<int> _widths;

        public Table(int count = 10)
        {
            Items = new List<T>();
            _count = count;
            _offset = 0;
            _selectedIndex = 0;
            _headers = ExtractPropertyNames(typeof(T));
            _widths = _headers.Select(h => h.Length).ToList();
        }

        public override void Render(bool selected)
        {
            if (_selectedIndex > Items.Count - 1)
                _selectedIndex = Items.Count - 1;

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int leftWidth = width / 2;

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            List<List<string>> rows = Items
                    .Select(item => ExtractPropertyValues(typeof(T), item))
                    .ToList();

            CalculateWidths(rows);

            RenderRow(null, '+', '-', selected, ConsoleColor.Red);
            RenderRow(_headers, '+', ' ', selected, ConsoleColor.Red);
            RenderRow(null, '+', '=', selected, ConsoleColor.Red);

            for (int i = _offset; i < _offset + _count; i++)
            {
                if (i < Items.Count)
                {
                    bool selectedRow = (Active && i == _selectedIndex);
                    RenderRow(rows[i], '|', ' ', selectedRow, ConsoleColor.Green);
                }
                else
                {
                    RenderRow(null, '|', ' ', false, ConsoleColor.White);
                }
            }

            RenderRow(null, '+', '-', selected, ConsoleColor.Red);

            Console.ResetColor();
        }

        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (_selectedIndex > Items.Count - 1)
                _selectedIndex = Items.Count - 1;

            if (!Active) return;

            if (keyInfo.Key == ConsoleKey.UpArrow && _selectedIndex > 0)
            {
                _selectedIndex--;

                if (_selectedIndex == _offset - 1)
                    _offset--;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow && _selectedIndex < Items.Count - 1)
            {
                _selectedIndex++;

                if (_selectedIndex == _offset + _count)
                    _offset++;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                ItemSelected?.Invoke();
            }
        }

        private void RenderRow(List<string>? values, char sep, char pad, bool selected, ConsoleColor color)
        {
            if (selected)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            for (int i = 0; i < _widths.Count; i++)
            {
                string value = values != null ? values[i] : string.Empty;
                string text = value.PadRight(_widths[i], pad);
                ConsoleHelper.WriteConditionalColor($"{sep}{pad}{text}{pad}", selected, color);
            }
            ConsoleHelper.WriteLineConditionalColor($"{sep}", selected, color);

            Console.ResetColor();
        }

        private void CalculateWidths(List<List<string>> rows)
        {
            for (int i = 0; i < _widths.Count; i++)
            {
                foreach (List<string> row in rows)
                {
                    if (row[i].Length > _widths[i])
                        _widths[i] = row[i].Length;
                }
            }
        }

        private List<string> ExtractPropertyNames(Type type)
        {
            return type
                .GetProperties()
                .Select(p => p.Name)
                .ToList();
        }

        private List<string> ExtractPropertyValues(Type type, object? obj)
        {
            return type
                .GetProperties()
                .Select(p => p.GetValue(obj)?.ToString() ?? string.Empty)
                .ToList();
        }
        public List<string> GetLines(bool selected)
        {
            if (_selectedIndex > Items.Count - 1)
                _selectedIndex = Items.Count - 1;

            List<List<string>> rows = Items
                .Select(item => ExtractPropertyValues(typeof(T), item))
                .ToList();

            CalculateWidths(rows);

            List<string> lines = new List<string>();

            lines.Add(RenderRowToString(null, '+', '-', selected));

            for (int i = _offset; i < _offset + _count; i++)
            {
                if (i < Items.Count)
                {
                    bool selectedRow = Active && i == _selectedIndex;
                    lines.Add(RenderRowToString(rows[i], '|', ' ', selectedRow));
                }
                else
                {
                    lines.Add(RenderRowToString(null, '|', ' ', false));
                }
            }

            lines.Add(RenderRowToString(null, '+', '-', selected));

            return lines;
        }
        private string RenderRowToString(List<string>? values, char sep, char pad, bool selected)
        {
            string line = "";

            for (int i = 0; i < _widths.Count; i++)
            {
                string value = values != null ? values[i] : string.Empty;
                string text = value.PadRight(_widths[i], pad);
                line += $"{sep}{pad}{text}{pad}";
            }

            line += sep;

            return line;
        }
        public void StretchToWidth(int totalWidth)
        {
            int columns = _widths.Count;

            int padding = columns * 3 + 1;
            int available = totalWidth - padding;

            int colWidth = available / columns;

            for (int i = 0; i < _widths.Count; i++)
            {
                _widths[i] = colWidth;
            }
        }
        public void MoveUp()
        {
            if (Items.Count == 0) return;

            _selectedIndex = (_selectedIndex - 1 + Items.Count) % Items.Count;
        }
        public void MoveDown()
        {
            if (Items.Count == 0) return;

            _selectedIndex = (_selectedIndex + 1) % Items.Count;
        }
    }
}
