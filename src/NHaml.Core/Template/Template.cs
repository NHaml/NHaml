using System;
using System.IO;
using NHaml.Core.Utils;

namespace NHaml.Core.Template
{
    public abstract class Template
    {
        protected Template()
        {
            Child = null;
        }

        protected Template(Template child)
        {
            Child = child;
        }

        public abstract bool ContainsContent(string name);
        public abstract void RunContent(TextWriter textWriter, string name);

        public virtual void Render(TextWriter textWriter)
        {
            RunContent(textWriter, "Main");
        }

        public Template Child { get; set; }
    }
}