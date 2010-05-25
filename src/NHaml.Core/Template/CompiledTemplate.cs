using System;
using System.Collections.Generic;
using NHaml.Core.Exceptions;
using NHaml.Core.Utils;
using NHaml.Core.Ast;
using NHaml.Core.TemplateResolution;

namespace NHaml.Core.Template
{
    public class CompiledTemplate
    {
        private readonly TemplateOptions options;
        private readonly Type _templateBaseType;
        private readonly object context;
        private TemplateFactory _templateFactory;
        private CompiledTemplate _masterFile;
        private IViewSource _contentFile;
        private readonly object _sync = new object();
        private IList<Func<bool>> viewSourceModifiedChecks;

        internal CompiledTemplate(TemplateOptions options, Type templateBaseType, object context, CompiledTemplate masterFile, IViewSource contentFile )
        {
            this.options = options;
            _templateBaseType = templateBaseType;
            this.context = context;
            _masterFile = masterFile;
            _contentFile = contentFile;
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
            var className = Utility.MakeClassName(_contentFile.Path);
            var compiler = options.GetTemplateCompiler();

            Template masterTemplate = null;
            if (_masterFile != null) {
                masterTemplate = _masterFile.CreateInstance();
            }

            compiler.SetDocument(_contentFile.ParseResult, className);
            
            if (_templateBaseType.IsGenericTypeDefinition)
            {
                var modelType = GetModelType(compiler.Document);
                options.TemplateBaseType = _templateBaseType.MakeGenericType(modelType);
                options.AddReference(modelType.Assembly);
            }
            else
            {
                options.TemplateBaseType = _templateBaseType;
            }
            compiler.GenerateType(options);

            viewSourceModifiedChecks = new List<Func<bool>>();
            viewSourceModifiedChecks.Add(() => _contentFile.IsModified);
            foreach (var fileModifiedCheck in _masterFile.viewSourceModifiedChecks)
                viewSourceModifiedChecks.Add(fileModifiedCheck);

            options.AddReferences(_templateBaseType);
            if (options.BeforeCompile != null)
            {
                options.BeforeCompile(compiler, context);
            }

            _templateFactory = new TemplateFactory(compiler.GenerateType(options));
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