using System;
using System.IO;

using NHaml.Utils;

namespace NHaml
{
    public abstract class Template
    {
        private readonly OutputWriter _outputWriter = new OutputWriter();

        public void Render( TextWriter textWriter )
        {
            Invariant.ArgumentNotNull( textWriter, "textWriter" );

            _outputWriter.TextWriter = textWriter;

            PreRender( _outputWriter );
            CoreRender( textWriter );
        }

        protected virtual void PreRender( OutputWriter outputWriter )
        {
        }

        protected virtual void CoreRender( TextWriter textWriter )
        {
        }

        protected OutputWriter Output
        {
            get { return _outputWriter; }
        }

        protected void RenderAttributeIfValueNotNull(TextWriter textWriter, string attributeSchema, string attributeName, string attributeValue)
        {
            var asString = Convert.ToString(attributeValue);
            if (asString != null)
            {
                if (attributeSchema.Length == 0)
                {
                    textWriter.Write(@" {0}=""", attributeName);
                }
                else
                {
                    textWriter.Write(@" {0}:{1}=""", attributeSchema, attributeName);
                }
                textWriter.Write(asString);
                textWriter.Write(@"""");
            }
        }
    }
}