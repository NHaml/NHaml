using System;
using Castle.MonoRail.Framework;

namespace NHaml.Samples.MonoRail.Components
{
    public class DateComponent : ViewComponent
    {
        public override void Render()
        {
            RenderText(DateTime.Now.ToString());
        }
    }

}
