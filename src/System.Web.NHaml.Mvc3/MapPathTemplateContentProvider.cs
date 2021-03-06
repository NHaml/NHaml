﻿using System.Web.NHaml.TemplateResolution;
using System.Web.Routing;
using System.IO;

namespace System.Web.NHaml.Mvc3
{
    public class MapPathTemplateContentProvider : FileTemplateContentProvider
    {
        private RequestContext _context;

        public void SetRequestContext(RequestContext context)
        {
            _context = context;
        }

        protected override FileInfo CreateFileInfo(string templateName)
        {
            var info = base.CreateFileInfo(templateName);
            if ((info != null) || (_context == null))
            {
                return info;
            }
            else
            {
                string path = _context.HttpContext.Request.MapPath(templateName);
                return base.CreateFileInfo(path);
            }
        }
    }
}
