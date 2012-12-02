using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using NHaml.Crosscutting;

namespace NHaml.TemplateResolution
{
    public class FileTemplateContentProvider : ITemplateContentProvider
    {
        private readonly List<string> _pathSources;

        public FileTemplateContentProvider()
        {
            _pathSources = new List<string> ();
            AddPathSource("Views");
        }

        public ViewSource GetViewSource(string templateName)
        {
            return GetViewSource(templateName, new List<ViewSource>());
        }

        public ViewSource GetViewSource(string templateName, IEnumerable<ViewSource> parentViewSourceList)
        {
            Invariant.ArgumentNotEmpty(templateName, "templateName");
            Invariant.ArgumentNotNull(parentViewSourceList, "parentViewSourceList");
            
            templateName = SuffixWithHaml(templateName);
            var fileInfo = CreateFileInfo(templateName);
            if (fileInfo != null && fileInfo.Exists)
                return new FileViewSource(fileInfo);

            //foreach (var source in parentViewSourceList)
            //{
            //    //search where the current parent template exists
            //    var parentDirectory = Path.GetDirectoryName(source.FilePath);
            //    var combine = Path.Combine(parentDirectory, templateName);
            //    if (File.Exists(combine))
            //        return new FileViewSource(new FileInfo(combine));
            //}

            throw new FileNotFoundException(string.Format("Could not find template '{0}'.", templateName));
        }

        protected virtual FileInfo CreateFileInfo(string templateName)
        {
            return PathSources
                .Select(pathSource => CreateFileInfo(pathSource, templateName))
                .FirstOrDefault(fileInfo => fileInfo.Exists);
        }

        private static string SuffixWithHaml(string templateName)
        {
            return templateName.EndsWith(".haml")
                ? templateName
                : templateName + ".haml";
        }


        public IEnumerable<string> PathSources {
            get
            {
                return new ReadOnlyCollection<string>(_pathSources);
            }
        }

        /// <remarks>The path is assumed to be relative to the AppDoamin BaseDirectory.</remarks>
        public void AddPathSource(string pathSource)
        {
            _pathSources.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pathSource));
        }

        private static FileInfo CreateFileInfo(string viewRoot, string templateName)
        {
            var info = new FileInfo(templateName);
            return info.Exists
                ? info
                : new FileInfo(Path.Combine(viewRoot, templateName));
        }

    }
}