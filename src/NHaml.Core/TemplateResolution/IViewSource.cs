using System.IO;
using NHaml.Core.Ast;

namespace NHaml.Core.TemplateResolution
{
    /// <summary>
    /// Represents a view template source
    /// </summary>
    public interface IViewSource
    {
        DocumentNode ParseResult { get; }

        /// <summary>
        /// Opens the view stream.
        /// </summary>
        /// <returns></returns>
        StreamReader GetStreamReader();

        string Path { get;  }

        bool IsModified { get; }
    }
}