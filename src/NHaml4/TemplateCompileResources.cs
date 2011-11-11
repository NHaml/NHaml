using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.TemplateResolution;

namespace NHaml
{
    public class TemplateCompileResources
    {
        private TemplateOptions _templateOptions;
        private Type _templateBaseType;
        private IList<TemplateResolution.IViewSource> _templateViewSources;
        private IList<string> _templatePaths;
        
        public Type TemplateBaseType {
            get { return _templateBaseType; }
        }
        //public object context { get; set; }

        public TemplateCompileResources(Type templateBaseType, IList<IViewSource> viewSources)
        {
            _templateBaseType = templateBaseType;
            _templateViewSources = viewSources;
        }

        public TemplateCompileResources(Type templateBaseType, IList<string> templatePaths)
        {
            _templateBaseType = templateBaseType;
            _templatePaths = templatePaths;
        }

        public TemplateCompileResources(Type templateBaseType, string templatePath)
            : this(templateBaseType, new List<string> { templatePath })
        { }

        public IList<IViewSource> GetViewSources(ITemplateContentProvider contentProvider)
        {
            if (_templateViewSources == null)
            {
                _templateViewSources = new List<IViewSource>();
                foreach (var layoutTemplatePath in _templatePaths)
                {
                    _templateViewSources.Add(contentProvider.GetViewSource(layoutTemplatePath));
                }
            }

            return _templateViewSources;
        }
    }
}
