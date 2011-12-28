using System.Collections.Generic;
using NHaml4.TemplateResolution;
using System.IO;

namespace NHaml4
{
    public class ViewSourceList : List<IViewSource>
    {
        public ViewSourceList()
        { }

        public ViewSourceList(FileInfo fileInfo)
        {
            this.Add(new FileViewSource(fileInfo));}
    }
}
