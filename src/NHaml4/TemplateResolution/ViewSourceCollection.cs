using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.TemplateResolution
{
    public class ViewSourceCollection : List<IViewSource>
    {
        public string GetClassName()
        {
            string result = "";

            if (this.Any())
                result += this[0].GetClassName();

            return result;
        }

        internal IViewSource GetByPartialName(string partialName)
        {
            try
            {
                return this.First(x => x.FileName.ToLower() == partialName.ToLower());
            }
            catch (InvalidOperationException)
            {
                throw new PartialNotFoundException(partialName);
            }
        }
    }
}
