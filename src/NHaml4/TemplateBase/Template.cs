using System;
using System.IO;
using NHaml4.Crosscutting;

namespace NHaml4.TemplateBase
{
    public abstract class Template
    {
        public void Render(TextWriter textWriter)
        {
            Invariant.ArgumentNotNull(textWriter, "textWriter");

            CoreRender(textWriter);
        }

        protected virtual void CoreRender(TextWriter textWriter)
        {
        }
    }
}
