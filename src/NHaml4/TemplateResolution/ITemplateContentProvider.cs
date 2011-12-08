using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHaml4.TemplateResolution
{
    public interface ITemplateContentProvider
    {
        /// <summary>
        /// Builds and returns a representation of a view template
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <returns></returns>
        IViewSource GetViewSource(string templateName);

        /// <summary>
        /// Builds and returns a representation of a view template
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <param name="parentViewSourceList">The parent view source list.</param>
        /// <returns></returns>
        IViewSource GetViewSource( string templatePath, IList<IViewSource> parentViewSourceList );

        /// <summary>
        /// Gets a list of path sources
        /// </summary>
        /// <value></value>
        ReadOnlyCollection<string> PathSources { get; }

        /// <summary>
        /// Adds the path source.
        /// </summary>
        /// <param name="pathSource">The path source.</param>
        void AddPathSource(string pathSource);
    }
}