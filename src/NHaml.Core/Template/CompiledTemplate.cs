using System;
using System.Collections.Generic;
using NHaml.Core.Exceptions;
using NHaml.Core.Utils;
using NHaml.Core.Ast;

namespace NHaml.Core.Template
{
    public class CompiledTemplate
    {
        private readonly TemplateOptions options;
        private readonly Type _templateBaseType;
        private readonly object context;
        private TemplateFactory _templateFactory;
        private readonly object _sync = new object();
        private IList<Func<bool>> viewSourceModifiedChecks;

        internal CompiledTemplate(TemplateOptions options, Type templateBaseType, object context)
        {
            this.options = options;
            _templateBaseType = templateBaseType;
            this.context = context;

            Compile(context);
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
                        Compile(context);
                        break;
                    }
                }
            }
        }

        private void Compile(object context)
        {
            /*var className = Utility.MakeClassName(ListExtensions.Last(_layoutViewSources).Path);
            var compiler = options.TemplateCompiler;


            var templateClassBuilder = compiler.CreateTemplateClassBuilder(className);

            var templateParser = new TemplateParser(options, templateClassBuilder, _layoutViewSources);

            var viewSourceReader = templateParser.Parse();
            viewSourceModifiedChecks = viewSourceReader.ViewSourceModifiedChecks;

            if (_templateBaseType.IsGenericTypeDefinition)
            {
                var modelType = GetModelType(compiler.Document);
                templateClassBuilder.BaseType = _templateBaseType.MakeGenericType(modelType);
                options.AddReference(modelType.Assembly);
            }
            else
            {
                templateClassBuilder.BaseType = _templateBaseType;
            }
            templateParser.Options.AddReferences(_templateBaseType);
            if (options.BeforeCompile != null)
            {
                options.BeforeCompile(templateClassBuilder, context);
            }

            _templateFactory = compiler.Compile(viewSourceReader, templateParser.Options, templateClassBuilder);

            */
        }

        private static Type GetModelType(DocumentNode n)
        {
            List<MetaNode> modelList;
            if (n.Metadata.TryGetValue("model", out modelList))
            {
                string model = modelList[0].Value;
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