using Castle.MonoRail.Framework;

namespace NHaml.Web.MonoRail.Tests.Components
{
    public class RenderParametersComponent : ViewComponent
    {
        public override void Render()
        {
            RenderText((string)Context.ComponentParameters["key1"]);
            RenderText( Context.ComponentParameters["key2"].ToString());
            RenderText( (string) Context.ComponentParameters["key3"]);
        }
    }
}