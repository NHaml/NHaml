using Castle.MonoRail.Framework;

namespace NHaml.Web.MonoRail.Tests.Components
{
	public class PropertyBagComponent : ViewComponent
    {
        public override void Render()
        {
            PropertyBag["value"] = "Foo";
            base.Render();
        }
	}
}
