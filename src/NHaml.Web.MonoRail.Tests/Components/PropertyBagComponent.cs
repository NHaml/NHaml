using System;
using Castle.MonoRail.Framework;

namespace NHaml.Web.MonoRail.Tests.Components
{
	public class PropertyBagComponent : ViewComponent
    {

        public PropertyBagComponent()
        {
            AttProperty = String.Empty;
        }
        public override void Render()
        {
            PropertyBag["PropertyBagComponent_Style"] = AttProperty;
            PropertyBag["value"] = "Foo";
            base.Render();
        }

        [ViewComponentParam("attProperty")]
        public string AttProperty { get; set; }
      
	}
}
