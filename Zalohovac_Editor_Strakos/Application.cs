using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zalohovac_Editor_Strakos.Presentation.Windows;

namespace Zalohovac_Editor_Strakos
{
    public class Application
    {
        private bool _running;
        private IWindow? _activeWindow;

        public Application()
        {
            _running = false;
            _activeWindow = null;
        }

        public void Run(IWindow window)
        {
            _running = true;
            _activeWindow = window;

            Console.Title = "BackupClient";
            Console.CursorVisible = false;
            Console.Clear();

            while (_running)
            {
                Render();
                HandleKey(Console.ReadKey(true));
            }
        }
        public void Stop()
        {
            _running = false;
        }
        public void SwitchWindow(IWindow window)
        {
            Console.Clear();
            _activeWindow = window;
        }
        public void Render()
        {
            Console.SetCursorPosition(0, 0);
            _activeWindow?.Render();
        }
        public void HandleKey(ConsoleKeyInfo keyInfo)
        {
            _activeWindow?.HandleKey(keyInfo);
        }
    }
}
