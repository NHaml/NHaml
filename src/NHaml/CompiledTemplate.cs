using System;
using System.Collections.Generic;
using NHaml.Exceptions;
using NHaml.TemplateResolution;
using NHaml.Utils;
using NHaml.Parser;

namespace NHaml
{
    public class CompiledTemplate
    {
        private readonly TemplateCompileResources _resources;
        private readonly TemplateOptions _options;
        private TemplateFactory _templateFactory;
        private readonly object _sync = new object();
        private IList<Func<bool>> _viewSourceModifiedChecks;
        private readonly ITreeParser _treeParser;

        public CompiledTemplate(TemplateOptions options, TemplateCompileResources resources, ITreeParser treeParser)
        {
            _options = options;
            _treeParser = treeParser;
            _resources = resources;
        }

        public Template CreateInstance()
        {
            return _templateFactory.CreateTemplate();
        }

        public void Recompile()
        {
            lock (_sync)
            {
                foreach (var fileModifiedCheck in _viewSourceModifiedChecks)
                {
                    if (fileModifiedCheck())
                    {
                        Compile();
                        break;
                    }
                }
            }
        }

        public void CompileTemplateFactory()
        {
            var templateSource = GetTemplate();
            //var classBuilder = new ClassBuilder();
            //classBuilder.Build(template);
        }

        private HamlTree GetTemplate()
        {   
            var layoutViewSources = _resources.GetViewSources(_options.TemplateContentProvider);
            return _treeParser.Parse(layoutViewSources);
        }

        public void Compile()
        {
            var layoutViewSources = _resources.GetViewSources(_options.TemplateContentProvider);
            string className = Utility.MakeClassName(ListExtensions.Last(layoutViewSources).Path);

            var viewSourceReader = new ViewSourceReader(_options, layoutViewSources);
            var templateClassBuilder = _options.TemplateCompiler.CreateTemplateClassBuilder(className);

            var templateParser = new TemplateParser(_options);
            templateParser.Parse(viewSourceReader, templateClassBuilder);

            _viewSourceModifiedChecks = viewSourceReader.ViewSourceModifiedChecks;

            if(_resources.TemplateBaseType.IsGenericTypeDefinition )
            {
                var modelType = GetModelType(templateClassBuilder.Meta);
                templateClassBuilder.BaseType = _resources.TemplateBaseType.MakeGenericType(modelType);
                _options.AddReference( modelType.Assembly );
            }
            else
            {
                templateClassBuilder.BaseType = _resources.TemplateBaseType;
            }
            templateParser.Options.AddReferences(_resources.TemplateBaseType);

            _templateFactory = _options.TemplateCompiler.Compile(
                viewSourceReader, templateParser.Options, templateClassBuilder);
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