using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class FormBuilder<TModel>
    where TModel : IModel
  {
    private const string FormFormat = "<form action=\"{0}\" method=\"{1}\"{2}>";
    private const string LabelFormat = "<label for=\"{0}\">{1}</label>";

    private readonly TModel _model;

    private readonly HtmlHelper _html;
    private readonly IOutputWriter _outputWriter;

    private readonly string _modelName;

    public FormBuilder(TModel model, HtmlHelper html, IOutputWriter outputWriter)
    {
      _model = model;
      _html = html;
      _outputWriter = outputWriter;
      _modelName = _model.GetType().Name;
    }

    public virtual void OpenTag(FormMethod method, IDictionary<string, object> attributes)
    {
      var virtualPath = RouteTable.Routes.GetVirtualPath(_html.ViewContext,
        new RouteValueDictionary
          {
            {"controller", Inflector.Pluralize(_model.GetType().Name).ToLowerInvariant()},
            {"action", _model.IsNew ? "create" : "update"},
            {"id", _model.IsNew ? (object)null : _model.Id}
          });

      _outputWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, FormFormat,
        new object[]
          {
            (virtualPath != null) ? virtualPath.VirtualPath : null,
            Enum.GetName(typeof(FormMethod), method).ToLowerInvariant(),
            ConvertObjectToAttributeList(attributes)
          }));
    }

    public virtual void CloseTag()
    {
      _outputWriter.WriteLine("</form>");
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual string HiddenField<TValue>(Expression<Func<TModel, TValue>> target)
    {
      var property = UnpickPropertyExpression(_model, target);

      return _html.Hidden(property.BindingName, property.Value);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual string Label<TValue>(Expression<Func<TModel, TValue>> target)
    {
      var property = UnpickPropertyExpression(_model, target);

      return string.Format(CultureInfo.InvariantCulture, LabelFormat,
        property.BindingName, Inflector.Titleize(property.Name));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual string TextField<TValue>(Expression<Func<TModel, TValue>> target)
    {
      var property = UnpickPropertyExpression(_model, target);

      return _html.TextBox(property.BindingName, property.Value);
    }

    private PropertyBinding UnpickPropertyExpression<TProperty>(
      TModel model, Expression<Func<TModel, TProperty>> expression)
    {
      var propertyData = new PropertyBinding();

      if (expression == null)
      {
        return propertyData;
      }

      var memberExpression = expression.Body as MemberExpression;

      if (memberExpression == null)
      {
        return propertyData;
      }

      var propertyInfo = memberExpression.Member as PropertyInfo;

      if (propertyInfo == null)
      {
        return propertyData;
      }

      propertyData.Name = propertyInfo.Name;
      propertyData.BindingName = _modelName + "." + propertyInfo.Name;

      var value = propertyInfo.GetValue(model, null);

      propertyData.Value = (value != null) ? value.ToString() : null;

      return propertyData;
    }

    private static string ConvertObjectToAttributeList(object obj)
    {
      var builder = new StringBuilder();

      if (obj != null)
      {
        var dictionary = (obj as IDictionary<string, object>) ?? new RouteValueDictionary(obj);

        const string format = " {0}=\"{1}\"";

        foreach (var key in dictionary.Keys)
        {
          var value = dictionary[key];

          if (dictionary[key] is bool)
          {
            value = dictionary[key].ToString().ToLowerInvariant();
          }

          builder.AppendFormat(format, key.ToLowerInvariant(), value);
        }
      }

      return builder.ToString();
    }
  }
}