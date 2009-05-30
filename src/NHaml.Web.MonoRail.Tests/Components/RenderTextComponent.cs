using Castle.MonoRail.Framework;

namespace NHaml.Web.MonoRail.Tests.Components
{
    public class RenderTextComponent : ViewComponent
    {
        public override void Render()
        {
            RenderText("RenderedText");
        }
    }
}