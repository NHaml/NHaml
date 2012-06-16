using System.IO;
using System;
using System.Text;

namespace NHaml4.TemplateResolution
{
    /// <summary>
    /// Represents a view template source
    /// </summary>
    public abstract class ViewSource
    {
        public abstract TextReader GetTextReader();
        public abstract string FilePath { get; }
        public abstract string FileName { get; }
        public abstract DateTime TimeStamp { get; }
        
        public string GetClassName()
        {
            string templatePath = FilePath;
            var stringBuilder = new StringBuilder();
            foreach (char ch in templatePath)
            {
                stringBuilder.Append(Char.IsLetter(ch) ? ch : '_');
            }

            return stringBuilder.ToString();
        }
    }
}