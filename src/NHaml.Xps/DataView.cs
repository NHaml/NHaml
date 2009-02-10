namespace NHaml.Xps
{
    public abstract class DataView<TViewData> : Template
    {
        public TViewData ViewData { get; set; }
    }
}