using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.TemplateResolution;

namespace NHaml4
{
    public interface IViewSourceList
    {
        void Add(IViewSource item);
        string GetPathName();
        IViewSource this[int index]
        {
            get;
        }

    }
}
