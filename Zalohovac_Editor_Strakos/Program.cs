using Zalohovac_Editor_Strakos.Presentation.Windows;

namespace Zalohovac_Editor_Strakos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Application app = new Application();

            IWindow window = new BackupJobWindow(app);

            app.Run(window);
        }
    }
}
