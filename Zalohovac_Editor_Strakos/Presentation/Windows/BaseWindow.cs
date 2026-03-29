using Zalohovac_Editor_Strakos.Presentation.Components;
using Zalohovac_Editor_Strakos.Presentation.ViewModels;

namespace Zalohovac_Editor_Strakos.Presentation.Windows
{
    public class BaseWindow : IWindow
    {
        public event Action? Closed;
        public event Action? Submitted;

        protected string _title;
        protected Application _application;
        protected IWindow? _returnWindow;

        private List<IComponent> _components;
        private int _selectedIndex;

        public BaseWindow(string title, Application application, IWindow? returnWindow = null)
        {
            _title = title;
            _application = application;
            _returnWindow = returnWindow;
            _components = new List<IComponent>();
            _selectedIndex = 0;
        }

        public virtual void Show()
        {
            _application.SwitchWindow(this);
        }

        public virtual void Render()
        {
            Console.Clear();

            Console.WriteLine($"=== {_title} ===");

            foreach (IComponent component in _components)
            {
                component.Render(component == _components[_selectedIndex]);
            }
        }

        public virtual void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                Close();
            }
            else if (keyInfo.Key == ConsoleKey.Tab)
            {
                do
                {
                    _selectedIndex = (_selectedIndex + 1) % _components.Count;
                } while (!_components[_selectedIndex].Selectable);
            }
            else
            {
                _components[_selectedIndex].HandleKey(keyInfo);
            }
        }

        protected void RegisterComponent(IComponent component)
        {
            _components.Add(component);

            if (!_components[_selectedIndex].Selectable)
                _selectedIndex++;
        }

        protected void Close()
        {
            Closed?.Invoke();
            _returnWindow?.Show();
        }
        protected void Submit()
        {
            Submitted?.Invoke();
            Close();
        }
    }
}
