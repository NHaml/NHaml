using System;
using System.Collections.Generic;
using NHaml.Core.Exceptions;
using NHaml.Core.Utils;
using NHaml.Core.Ast;
using NHaml.Core.TemplateResolution;
using System.CodeDom.Compiler;
using System.Text;

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
            Template t = _templateFactory.CreateTemplate();
            if (_masterFile != null)
                t.Master = _masterFile.CreateInstance();
            return t;
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

            compiler.SetDocument(options, _contentFile.ParseResult, className);

            viewSourceModifiedChecks = new List<Func<bool>>();
            viewSourceModifiedChecks.Add(() => _contentFile.IsModified);
            if (_masterFile != null)
            {
                foreach (var fileModifiedCheck in _masterFile.viewSourceModifiedChecks)
                {
                    viewSourceModifiedChecks.Add(fileModifiedCheck);
                }
            }

            options.AddReferences(_templateBaseType);
            if (options.BeforeCompile != null)
            {
                options.BeforeCompile(compiler, context);
            }

            Type generated = compiler.GenerateType(options);
            if (generated == null) {
                StringBuilder errorString = new StringBuilder(); ;
                foreach (CompilerError error in compiler.CompilerResults.Errors)
                {
                    errorString.AppendLine(error.ToString());
                }
                throw new TemplateCompilationException(errorString.ToString());
            }
            _templateFactory = new TemplateFactory(generated);
        }
    }
}