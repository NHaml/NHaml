using Castle.MonoRail.Framework;

namespace NHaml.Web.MonoRail.Tests
{
    public class RenderBodyComponent : ViewComponent
    {
        public override void Render()
        {
            Context.RenderBody();
        }
    }
}