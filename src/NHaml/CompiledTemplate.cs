using System;
using System.Collections.Generic;
using NHaml.Exceptions;
using NHaml.TemplateResolution;
using NHaml.Utils;

namespace NHaml
{
    public class CompiledTemplate
    {
        private readonly TemplateOptions options;
        private readonly IList<IViewSource> _layoutViewSources;
        private readonly Type _templateBaseType;
        private TemplateFactory _templateFactory;
        private readonly object _sync = new object();
        private IList<Func<bool>> viewSourceModifiedChecks;

        internal CompiledTemplate(TemplateOptions options, IList<IViewSource> layoutViewSources, Type templateBaseType)
        {
            this.options = options;
            _layoutViewSources = layoutViewSources;
            _templateBaseType = templateBaseType;

            Compile();
        }

        public Template CreateInstance()
        {
            return _templateFactory.CreateTemplate();
        }

        public void Recompile()
        {
            lock (_sync)
            {
                foreach (var fileModifiedCheck in viewSourceModifiedChecks)
                {
                    if (fileModifiedCheck())
                    {
                        Compile();
                        break;
                    }
                }
            }
        }

        private void Compile()
        {
            var className = Utility.MakeClassName( ListExtensions.Last(_layoutViewSources).Path );
            var compiler = options.TemplateCompiler;
            var templateClassBuilder = compiler.CreateTemplateClassBuilder(className, _templateBaseType );

            var templateParser = new TemplateParser(options, templateClassBuilder, _layoutViewSources);

            var viewSourceReader = templateParser.Parse();
            viewSourceModifiedChecks = viewSourceReader.ViewSourceModifiedChecks;

            if( _templateBaseType.IsGenericTypeDefinition )
            {
                var modelType = GetModelType(templateClassBuilder.Meta);
                templateClassBuilder.BaseType = _templateBaseType.MakeGenericType( modelType );
                options.AddReference( modelType.Assembly );
            }

            _templateFactory = compiler.Compile(viewSourceReader, templateParser.Options, templateClassBuilder);

         
        }

        private static Type GetModelType(IDictionary<string, string> meta)
        {
            string model;
            if (meta.TryGetValue("model", out model))
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var modelType = assembly.GetType(model, false, true);
                    if (modelType != null)
                    {
                        return modelType;
                    }
                }

                var message = string.Format("The given model type '{0}' was not found.", model);
                throw new TemplateCompilationException(message);
            }
            return typeof (object);
        }
    }
}