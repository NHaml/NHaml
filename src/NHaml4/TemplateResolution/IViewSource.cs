using System.IO;
using System;

namespace NHaml4.TemplateResolution
{
    /// <summary>
    /// Represents a view template source
    /// </summary>
    public interface IViewSource
    {
        StreamReader GetStreamReader();

        string Path { get;  }

        DateTime TimeStamp { get; }

        string GetClassName();
    }
}