using Castle.MonoRail.Framework;

namespace NHaml.Web.MonoRail.Tests.Components
{
    public class RenderBodyComponent : ViewComponent
    {
        public override void Render()
        {
            RenderBody();
        }
    }
}