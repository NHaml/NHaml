using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.TemplateResolution;
using NHaml.Utils;

namespace NHaml4
{
    public class ViewSourceList : List<IViewSource>, IViewSourceList
    {
        public string GetPathName()
        {
            string templatePath = this.Last().Path;
            var stringBuilder = new StringBuilder();
            for (int c = 0; c < templatePath.Length; c++)
            {
                char ch = templatePath[c];

                if ((ch >= 97 && ch <= 122) || (ch >= 65 && ch <= 90) || (ch >= 0 && ch <= 9))
                {
                    stringBuilder.Append(ch);
                }
                else
                {
                    stringBuilder.Append('_');
                }
            }

            return stringBuilder.ToString().Replace(templatePath, "_").Trim('_');
        }
    }
}
