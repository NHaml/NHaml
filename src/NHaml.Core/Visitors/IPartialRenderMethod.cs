using System;
using System.Collections.Generic;
using System.Text;

namespace NHaml.Core.Visitors
{
    public interface IPartialRenderMethod
    {
        object RenderMethod(string PartialName, object PartialObject);
    }
}
