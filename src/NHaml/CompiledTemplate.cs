using System;
using System.Collections.Generic;
using System.IO;

using NHaml.Utils;

namespace NHaml
{
    public class CompiledTemplate
    {
        private readonly TemplateEngine _templateEngine;

        private readonly string _templatePath;
        private readonly string _layoutTemplatePath;

        private readonly Type _templateBaseType;

        private readonly Dictionary<string, DateTime> _fileTimestamps
          = new Dictionary<string, DateTime>();

        private TemplateFactory _templateFactory;

        private readonly object _sync = new object();

        internal CompiledTemplate( TemplateEngine templateEngine, string templatePath,
          string layoutTemplatePath, Type templateBaseType )
        {
            _templateEngine = templateEngine;
            _templatePath = templatePath;
            _layoutTemplatePath = layoutTemplatePath;
            _templateBaseType = templateBaseType;

            Compile();
        }

        public Template CreateInstance()
        {
            return _templateFactory.CreateTemplate();
        }

        public void Recompile()
        {
            lock( _sync )
            {
                foreach( var inputFile in _fileTimestamps )
                {
                    if( File.GetLastWriteTime( inputFile.Key ) > inputFile.Value )
                    {
                        Compile();

                        break;
                    }
                }
            }
        }

        private void Compile()
        {
            var templateClassBuilder = _templateEngine.TemplateCompiler.CreateTemplateClassBuilder(
              Utility.MakeClassName( _templatePath ), _templateBaseType );

            var templateParser
              = new TemplateParser( _templateEngine, templateClassBuilder, _templatePath, _layoutTemplatePath );

            templateParser.Parse();

            _templateFactory = _templateEngine.TemplateCompiler.Compile( templateParser );

            foreach( var inputFile in templateParser.InputFiles )
            {
                _fileTimestamps[inputFile] = File.GetLastWriteTime( inputFile );
            }
        }
    }
}