using Castle.MonoRail.Framework;

namespace NHaml.Web.MonoRail.Tests
{
    public class RenderTextComponent : ViewComponent
    {
        public override void Render()
        {
            RenderText("RenderedText");
        }
    }
}