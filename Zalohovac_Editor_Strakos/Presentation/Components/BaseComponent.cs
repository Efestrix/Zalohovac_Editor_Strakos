using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zalohovac_Editor_Strakos.Presentation.Components
{
    public abstract class BaseComponent : IComponent
    {
        private bool _inline;
        public abstract bool Selectable { get; }

        protected BaseComponent(bool inline = false)
        {
            _inline = inline;
        }
        public virtual void Render(bool selected)
        {
            if (_inline)
                Console.Write(" ");
            else
                Console.WriteLine();
        }
        public virtual void HandleKey(ConsoleKeyInfo keyInfo)
        { }

    }
}
