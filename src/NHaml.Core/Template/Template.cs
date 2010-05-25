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
            Master = null;
        }

        protected Template(Template master)
        {
            Master = master;
        }

        public abstract bool ContainsContent(string name);
        public abstract void RunContent(TextWriter textWriter, string name);

        public virtual void Render(TextWriter textWriter)
        {
            if (Master == null)
            {
                RunContent(textWriter, "Main");
            }
            else
            {
                Master.Child = this;
                Master.RunContent(textWriter, "Main");
            }
        }

        public Template Master { get; set; }
        public Template Child { get; set; }
    }
}