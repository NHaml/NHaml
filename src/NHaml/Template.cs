using System;
using System.IO;

using NHaml.Utils;

namespace NHaml
{
    public abstract class Template
    {
        private readonly OutputWriter _outputWriter = new OutputWriter();

        public virtual void Render( TextWriter textWriter )
        {
            Invariant.ArgumentNotNull( textWriter, "textWriter" );

            _outputWriter.TextWriter = textWriter;

            PreRender( _outputWriter );
            CoreRender( textWriter );
        }

        protected virtual void PreRender( OutputWriter outputWriter )
        {
        }

        protected abstract void CoreRender( TextWriter textWriter );

        protected OutputWriter Output
        {
            get { return _outputWriter; }
        }
        protected void RenderAttributeIfValueNotNull(TextWriter textWriter, string attributeKey, string attributeValue)
        {
            var asString = Convert.ToString(attributeValue);
            if (asString != null)
            {
                textWriter.Write(@"{0}=""", attributeKey);
                textWriter.Write(asString);
                textWriter.Write(@"""");
            }
        }
    }
}