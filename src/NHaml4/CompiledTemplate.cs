using System;
using System.Collections.Generic;
using NHaml.Exceptions;
using NHaml4.TemplateResolution;
using NHaml.Utils;
using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4;
using NHaml4.Walkers;
using System.Linq;

namespace NHaml
{
    public class CompiledTemplate
    {
        private readonly ITemplateContentProvider _templateContentProvider;
        private readonly ITreeParser _treeParser;
        private readonly IHamlTreeWalker _treeWalker;
        private readonly ITemplateFactoryCompiler _templateFactoryCompiler;
        private TemplateFactory _templateFactory;

        public CompiledTemplate(ITreeParser treeParser,
            IHamlTreeWalker treeWalker, ITemplateFactoryCompiler templateCompiler)
        {
            _treeParser = treeParser;
            _treeWalker = treeWalker;
            _templateFactoryCompiler = templateCompiler;
        }

        public void CompileTemplateFactory(IViewSource viewSource)
        {
            CompileTemplateFactory(new List<IViewSource> { viewSource });
        }

        public void CompileTemplateFactory(IList<IViewSource> viewSourceList)
        {
            HamlDocument hamlDocument = _treeParser.ParseDocument(viewSourceList);
            string templateCode = _treeWalker.Walk(hamlDocument, Utility.MakeClassName(viewSourceList.Last().Path));
            _templateFactoryCompiler.Compile(templateCode);
        }

        public Template CreateInstance()
        {
            if (_templateFactory == null)
                throw new NullReferenceException("Attempting to create an instance of a compiled template using a NULL templateFactory.");
            return _templateFactory.CreateTemplate();
        }

        //public void Recompile()
        //{
        //    lock (_sync)
        //    {
        //        foreach (var fileModifiedCheck in _viewSourceModifiedChecks)
        //        {
        //            if (fileModifiedCheck())
        //            {
        //                Compile();
        //                break;
        //            }
        //        }
        //    }
        //}

        //public void Compile()
        //{
        //    var layoutViewSources = _resources.GetViewSources(_options.TemplateContentProvider);
        //    string className = Utility.MakeClassName(ListExtensions.Last(layoutViewSources).Path);

        //    var viewSourceReader = new ViewSourceReader(_options, layoutViewSources);
        //    var templateClassBuilder = _options.TemplateCompiler.CreateTemplateClassBuilder(className);

        //    var templateParser = new TemplateParser(_options);
        //    templateParser.Parse(viewSourceReader, templateClassBuilder);

        //    _viewSourceModifiedChecks = viewSourceReader.ViewSourceModifiedChecks;

        //    if(_resources.TemplateBaseType.IsGenericTypeDefinition )
        //    {
        //        var modelType = GetModelType(templateClassBuilder.Meta);
        //        templateClassBuilder.BaseType = _resources.TemplateBaseType.MakeGenericType(modelType);
        //        _options.AddReference( modelType.Assembly );
        //    }
        //    else
        //    {
        //        templateClassBuilder.BaseType = _resources.TemplateBaseType;
        //    }
        //    templateParser.Options.AddReferences(_resources.TemplateBaseType);

        //    _templateFactory = _options.TemplateCompiler.Compile(
        //        viewSourceReader, templateParser.Options, templateClassBuilder);
        //}

    //    private static Type GetModelType(IDictionary<string, string> meta)
    //    {
    //        string model;
    //        if (meta.TryGetValue("model", out model))
    //        {
    //            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
    //            {
    //                var modelType = assembly.GetType(model, false, true);
    //                if (modelType != null)
    //                {
    //                    return modelType;
    //                }
    //            }

    //            var message = string.Format("The given model type '{0}' was not found.", model);
    //            throw new TemplateCompilationException(message);
    //        }
    //        return typeof (object);
    //    }

    }
}