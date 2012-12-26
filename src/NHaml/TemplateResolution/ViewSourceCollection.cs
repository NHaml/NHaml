using System.Collections.Generic;
using System.Linq;

namespace System.Web.NHaml.TemplateResolution
{
    public class ViewSourceCollection : List<ViewSource>
    {
        public string GetClassName()
        {
            var result = "";

            if (this.Any())
                result += this[0].GetClassName();

            return result;
        }

        internal ViewSource GetByPartialName(string partialName)
        {
            try
            {
                return string.IsNullOrEmpty(partialName)
                    ? this[1]
                    : this.First(x => x.FileName.ToLower() == partialName.ToLower());
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
