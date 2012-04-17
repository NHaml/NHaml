using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.TemplateResolution
{
    public class ViewSourceCollection : IEnumerable<IViewSource>
    {
        private IList<IViewSource> _viewSourceList = new List<IViewSource>();

        public void Add(IViewSource viewSource)
        {
            _viewSourceList.Add(viewSource);    
        }

        public string GetClassName()
        {
            string result = "";

            if (_viewSourceList.Any())
                result += _viewSourceList[0].GetClassName();

            return result;
        }

        public IEnumerator<IViewSource> GetEnumerator()
        {
            return _viewSourceList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _viewSourceList.GetEnumerator();
        }

        internal IViewSource GetByPartialName(string partialName)
        {
            return this.First(x => x.FileName.ToLower() == partialName.ToLower());
        }
    }
}
