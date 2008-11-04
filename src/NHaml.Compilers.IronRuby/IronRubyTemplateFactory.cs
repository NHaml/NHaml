using System.IO;

using Microsoft.Scripting.Hosting;

namespace NHaml.Compilers.IronRuby
{
  public class IronRubyTemplateFactory : TemplateFactory
  {
    private readonly ScriptEngine _scriptEngine;
    private readonly object _renderAction;

    public IronRubyTemplateFactory(ScriptEngine scriptEngine, string className)
    {
      _scriptEngine = scriptEngine;

      _renderAction = _scriptEngine.CreateScriptSourceFromString(
        className + ".new.method(:render)").Execute();
    }

    public ScriptEngine ScriptEngine
    {
      get { return _scriptEngine; }
    }

    public object RenderAction
    {
      get { return _renderAction; }
    }

    public override Template CreateTemplate()
    {
      return new DlrShimTemplate(_scriptEngine, _renderAction);
    }

    protected class DlrShimTemplate : Template
    {
      private readonly ScriptEngine _scriptEngine;
      private readonly object _renderAction;

      public DlrShimTemplate(ScriptEngine scriptEngine, object renderAction)
      {
        _scriptEngine = scriptEngine;
        _renderAction = renderAction;
      }

      public ScriptEngine ScriptEngine
      {
        get { return _scriptEngine; }
      }

      public object RenderAction
      {
        get { return _renderAction; }
      }

      public override void Render(TextWriter textWriter)
      {
        _scriptEngine.Operations.Call(_renderAction, textWriter);
      }

      protected override void CoreRender(TextWriter textWriter)
      {
      }
    }
  }
}