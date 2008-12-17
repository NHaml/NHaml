using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml.Xps
{
    public abstract class DataView<TViewData> : Template
    {
        public TViewData ViewData { get; set; }

    }
}