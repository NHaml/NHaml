using System;
using System.IO;
using NHaml4.Crosscutting;

namespace NHaml4.TemplateBase
{
    public abstract class Template
    {
        //protected Template()
        //{
        //    Output = new OutputWriter();
        //}

        // public OutputWriter Output { get; private set; }

        public void Render(TextWriter textWriter)
        {
            Invariant.ArgumentNotNull(textWriter, "textWriter");

            // Output.TextWriter = textWriter;

            PreRender(textWriter);
            CoreRender(textWriter);
            PostRender(textWriter);
        }

        protected virtual void PreRender(TextWriter outputWriter)
        {
        }

        protected virtual void CoreRender(TextWriter textWriter)
        {
        }

        protected virtual void PostRender(TextWriter outputWriter)
        {
        }

        protected void RenderAttributeIfValueNotNull(TextWriter textWriter, string attributeSchema, string attributeName, object attributeValue)
        {
            if (string.IsNullOrEmpty(attributeName) || attributeValue == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(attributeSchema)) attributeSchema += ":";
            var asString = Convert.ToString(attributeValue);

            textWriter.Write(" {0}{1}=\"{2}\"", attributeSchema, attributeName, asString);            
        }
    }
}