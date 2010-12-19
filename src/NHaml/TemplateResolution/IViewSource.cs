using System.IO;

namespace NHaml.TemplateResolution
{
    /// <summary>
    /// Represents a view template source
    /// </summary>
    public interface IViewSource
    {
        /// <summary>
        /// Opens the view.
        /// </summary>
        /// <returns></returns>
        TextReader GetReader();

        string Path { get;  }

        bool IsModified { get; }
    }
}