using Castle.MonoRail.Framework;

namespace NHaml.Samples.MonoRail.Components
{
	public class SimpleComponent : ViewComponent
	{
		public override void Render()
		{
			PropertyBag["value"] = "Foo";
			base.Render();
		}
	}
}