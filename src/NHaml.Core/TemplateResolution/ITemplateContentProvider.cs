using System.Collections.Generic;

namespace NHaml.Core.TemplateResolution
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
        /// <param name="parentViewSourceList">The list of paths where to search</param>
        /// <returns></returns>
        IViewSource GetViewSource(string templateName, IList<string> templatePaths);

        /// <summary>
        /// Gets a list of path sources
        /// </summary>
        /// <value></value>
        IList<string> PathSources { get; set; }

        /// <summary>
        /// Adds the path source.
        /// </summary>
        /// <param name="pathSource">The path source.</param>
        void AddPathSource(string pathSource);
    }
}