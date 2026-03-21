using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zalohovac_Editor_Strakos.Presentation.Windows
{
    public interface IWindow
    {
        event Action? Closed;
        event Action? Submitted;

        void Show();
        void Render();
        void HandleKey(ConsoleKeyInfo keyInfo);
    }
}
