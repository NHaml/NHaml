using System;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using NHaml.Engine;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class MvcCompiledView : CompiledView<IMvcView, ViewDataDictionary>
  {
    public MvcCompiledView(TemplateCompiler templateCompiler, string templatePath,
      string layoutPath, ViewDataDictionary viewData)
      : base(templateCompiler, templatePath, layoutPath, viewData)
    {
    }

    protected override Type[] GetGenericArguments(ViewDataDictionary viewData)
    {
      var modelType = typeof(object);

      if ((viewData != null) && (viewData.Model != null))
      {
        modelType = viewData.Model.GetType();
      }

      return new[] {modelType};
    }
  }
}