using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using NHaml4.Crosscutting;

namespace NHaml4.TemplateResolution
{
    public class FileTemplateContentProvider : ITemplateContentProvider
    {
        private readonly List<string> _pathSources;

        protected FileTemplateContentProvider()
        {
            _pathSources = new List<string> ();
            AddPathSource("Views");
        }

        public IViewSource GetViewSource(string templateName)
        {
            return GetViewSource(templateName, new List<IViewSource>());
        }

        public IViewSource GetViewSource(string templateName, IList<IViewSource> parentViewSourceList)
        {
            Invariant.ArgumentNotEmpty(templateName, "templateName");
            Invariant.ArgumentNotNull(parentViewSourceList, "parentViewSourceList");
            templateName = SuffixWithHaml(templateName);
            var fileInfo = CreateFileInfo(templateName);
            if (fileInfo != null && fileInfo.Exists)
            {
                return new FileViewSource(fileInfo);
            }
            for (var index = 0; index < parentViewSourceList.Count; index++)
            {
                var source = parentViewSourceList[index];
                //search where the current parent template exists
                var parentDirectory = Path.GetDirectoryName(source.Path);
                var combine = Path.Combine(parentDirectory, templateName);
                if (File.Exists(combine))
                {
                    return new FileViewSource(new FileInfo(combine));
                }
            }

            throw new FileNotFoundException(string.Format("Could not find template '{0}'.", templateName));
        }

        protected virtual FileInfo CreateFileInfo(string templateName)
        {

            foreach (var pathSource in PathSources)
            {
                var fileInfo = CreateFileInfo(pathSource, templateName);
                if (fileInfo.Exists)
                {
                    return fileInfo;
                }
            }

            return null;
        }

        private static string SuffixWithHaml(string templateName)
        {
            if (templateName.EndsWith(".haml"))
            {
                return templateName;
            }
            return templateName + ".haml";
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
   
//TODO:Not sure if this is useful
        //public bool HasSource(string sourceName)
        //{
        //    foreach (var pathSource in PathSources)
        //    {
        //        var fileInfo = CreateFileInfo(pathSource, sourceName);
        //        if (fileInfo.Exists)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        private static FileInfo CreateFileInfo(string viewRoot, string templateName)
        {
            //TODO: not sure what the purpose of this is. came from castle
            //if (Path.IsPathRooted(templateName))
            //{
            //    templateName = templateName.Substring(Path.GetPathRoot(templateName).Length);
            //}
            var info = new FileInfo(templateName);
            if (!info.Exists)
            {
                info = new FileInfo(Path.Combine(viewRoot, templateName));
            }

            return info;
        }

    }
}