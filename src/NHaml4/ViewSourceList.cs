using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.TemplateResolution;
using NHaml.Utils;
using System.IO;

namespace NHaml4
{
    public class ViewSourceList : List<IViewSource>
    {
        public ViewSourceList()
        { }

        public ViewSourceList(FileInfo fileInfo)
        {
            this.Add(new FileViewSource(fileInfo));
        }

        public string GetClassNameFromPathName()
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

        public string GetCacheKey()
        {
            return string.Join(",", this.Select(x => x.Path).ToArray());
        }
    }
}
