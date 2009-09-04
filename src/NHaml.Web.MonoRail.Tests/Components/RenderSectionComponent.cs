using Castle.MonoRail.Framework;

namespace NHaml.Web.MonoRail.Tests.Components
{
    public class RenderSectionComponent : ViewComponent
    {
        public override void Render()
        {
            RenderSection("Section1");
            RenderSection("Section2");
        }
    }
}