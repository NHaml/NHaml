using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.Core.TemplateResolution;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;

namespace NHaml.Web.Mvc
{
    public class MapPathTemplateContentProvider : FileTemplateContentProvider
    {
        private RequestContext _context;

        public MapPathTemplateContentProvider()
        {
        }

        public void SetRequestContext(RequestContext context)
        {
            _context = context;
        }

        protected override System.IO.FileInfo CreateFileInfo(string templateName)
        {
            var info = base.CreateFileInfo(templateName);
            if ((info == null) && (_context != null))
            {
                string path = _context.HttpContext.Request.MapPath(templateName);
                return base.CreateFileInfo(path);
            }
            else
            {
                return info;
            }
        }
    }
}
