using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zalohovac_Editor_Strakos.Presentation.Components
{
    public interface IComponent
    {
        bool Selectable { get; }

        public int X { get; set; }
        public int Y { get; set; }

        void Render(bool selected);
        void HandleKey(ConsoleKeyInfo keyInfo);
    }
}
