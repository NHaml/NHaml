using System.IO;

namespace NHaml4.TemplateResolution
{
    /// <summary>
    /// Represents a view template source
    /// </summary>
    public interface IViewSource
    {
        /// <summary>
        /// Opens the view stream.
        /// </summary>
        /// <returns></returns>
        StreamReader GetStreamReader();

        string Path { get;  }

        bool IsModified { get; }

        string GetClassName();
    }
}