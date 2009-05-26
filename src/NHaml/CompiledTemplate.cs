using System;
using System.Collections.Generic;
using System.IO;
using NHaml.Exceptions;
using NHaml.Utils;

namespace NHaml
{
    public class CompiledTemplate
    {
        private readonly TemplateEngine _templateEngine;

        private readonly string _templatePath;
        private readonly IList<string> _layoutTemplatePaths;

        private readonly Type _templateBaseType;

        private readonly Dictionary<string, DateTime> _fileTimestamps
          = new Dictionary<string, DateTime>();

        private TemplateFactory _templateFactory;

        private readonly object _sync = new object();

        internal CompiledTemplate( TemplateEngine templateEngine, string templatePath,
          IList<string> layoutTemplatePaths, Type templateBaseType )
        {
            _templateEngine = templateEngine;
            _templatePath = templatePath;
            _layoutTemplatePaths =  layoutTemplatePaths;
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
            var templateClassBuilder = _templateEngine.Options.TemplateCompiler.CreateTemplateClassBuilder(
              Utility.MakeClassName( _templatePath ), _templateBaseType );

            var templateParser = new TemplateParser(_templateEngine, templateClassBuilder,
                                                               _layoutTemplatePaths, _templatePath);

            templateParser.Parse();

            if( _templateBaseType.IsGenericTypeDefinition )
            {
                string model;
                Type modelType = null;

                if( templateParser.Meta.TryGetValue( "model", out model ) )
                {
                    foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
                    {
                        modelType = assembly.GetType( model, false, true );

                        if( modelType != null )
                            break;
                    }

                    if( modelType == null )
                        throw new TemplateCompilationException( string.Format(
                                                                   "The given model type '{0}' was not found.", model ) );
                }
                else
                    modelType = typeof (object);

                templateClassBuilder.BaseType = _templateBaseType.MakeGenericType( modelType );

                _templateEngine.Options.AddReference( modelType.Assembly );
            }

            _templateFactory = _templateEngine.Options.TemplateCompiler.Compile( templateParser );

            foreach( var inputFile in templateParser.InputFiles )
            {
                _fileTimestamps[inputFile] = File.GetLastWriteTime( inputFile );
            }
        }
    }
}