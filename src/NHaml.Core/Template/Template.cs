using System;
using System.IO;
using NHaml.Core.Utils;

namespace NHaml.Core.Template
{
    public abstract class Template
    {
        protected Template()
        {
        }

        public void Render(TextWriter textWriter)
        {
            CoreRender(textWriter);
        }

        public virtual void CoreRender(TextWriter textWriter)
        {
        }

    }
}