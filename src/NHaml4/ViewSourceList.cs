using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.TemplateResolution;
using NHaml.Utils;

namespace NHaml4
{
    public class ViewSourceList : List<IViewSource>
    {
        public string GetPathName()
        {
            string templatePath = this.Last().Path;
            var stringBuilder = new StringBuilder();
            for (int c = 0; c < templatePath.Length; c++)
            {
                char ch = templatePath[c];
                stringBuilder.Append(Char.IsLetter(ch) ? ch : '_');
            }

            return stringBuilder.ToString();
        }
    }
}
