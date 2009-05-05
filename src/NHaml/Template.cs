using System;
using System.IO;
using NHaml.Utils;

namespace NHaml
{
    public abstract class Template
    {
        protected Template()
        {
            Output = new OutputWriter();
        }

        public OutputWriter Output { get; private set; }

        public void Render(TextWriter textWriter)
        {
            Invariant.ArgumentNotNull(textWriter, "textWriter");

            Output.TextWriter = textWriter;

            PreRender(Output);
            CoreRender(textWriter);
            PostRender(Output);
        }

        protected virtual void PreRender(OutputWriter outputWriter)
        {
        }

        protected virtual void CoreRender(TextWriter textWriter)
        {
        }

        protected virtual void PostRender(OutputWriter outputWriter)
        {
        }

        protected void RenderAttributeIfValueNotNull(TextWriter textWriter, string attributeSchema, string attributeName,
                                                     object attributeValue)
        {
            if (attributeValue != null)
            {
                var asString = Convert.ToString(attributeValue);

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