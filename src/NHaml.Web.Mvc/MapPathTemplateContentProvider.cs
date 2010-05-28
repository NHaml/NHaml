using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.Core.TemplateResolution;
using System.Web;
using System.Web.Hosting;

namespace NHaml.Web.Mvc
{
    public class MapPathTemplateContentProvider : FileTemplateContentProvider
    {
        public MapPathTemplateContentProvider()
        {
        }

        protected override System.IO.FileInfo CreateFileInfo(string templateName)
        {
            var info = base.CreateFileInfo(templateName);
            if (info == null)
            {
                var file = HostingEnvironment.VirtualPathProvider.GetFile(templateName);
                return base.CreateFileInfo(file.VirtualPath);
            }
            else
            {
                return info;
            }
        }
    }
}
