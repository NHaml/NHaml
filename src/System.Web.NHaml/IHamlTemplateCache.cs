namespace System.Web.NHaml
{
    public interface IHamlTemplateCache
    {
        TemplateFactory GetOrAdd(string templateKey, DateTime timeStamp, Func<TemplateFactory> templateGet);
        void Clear();
    }
}
