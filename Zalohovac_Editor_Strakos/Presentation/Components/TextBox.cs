using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Helpers;

namespace Zalohovac_Editor_Strakos.Presentation.Components
{
    public class TextBox : BaseComponent
    {
        public override bool Selectable => true;
        public string Value { get; set; }

        private string _text;
        private int _size;

        public TextBox(string text, int size = 32, bool inline = false)
            : base(inline)
        {
            Value = string.Empty;
            _text = text;
            _size = size;
        }

        public override void Render(bool selected)
        {
            if (selected)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
            }

            char pad = selected ? '_' : ' ';
            string content = (Value ?? "").PadRight(_size, pad);

            ConsoleHelper.WriteConditionalColor($"{_text}{content}", selected, ConsoleColor.Red);

            base.Render(selected);

            Console.ResetColor();
        }

        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Backspace && Value.Length > 0)
            {
                Value = Value.Substring(0, Value.Length - 1);
            }
            else if (!char.IsControl(keyInfo.KeyChar) && Value.Length < _size)
            {
                Value += keyInfo.KeyChar;
            }
            else if (keyInfo.Key == ConsoleKey.Delete)
            {
                Value = string.Empty;
            }
        }
    }
}
