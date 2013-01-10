using System.Collections.Generic;

namespace System.Web.NHaml.TemplateBase
{
    public static class DocTypeFactory
    {
        public static string GetDocType(string docType, HtmlVersion htmlVersion)
        {
            var docTypeParts = docType.Split(' ');

            if (docTypeParts.Length == 0)
                return GetTransitionalDocType(htmlVersion);

            if (htmlVersion == HtmlVersion.Html5
                && docTypeParts[0].ToUpper() != "XML")
                return GetHtml5DocType();

            switch (docTypeParts[0].ToUpper())
            {
                case "STRICT": return GetStrictDocType(htmlVersion);
                case "FRAMESET": return GetFramesetDocType(htmlVersion);
                case "5": return GetHtml5DocType();
                case "1.1": return GetXHtmlDocType11();
                case "BASIC": return GetBasicDocType();
                case "MOBILE": return GetMobileDocType();
                case "RDFA": return GetRdfaDocType();
                case "XML": return GetXmlDocType(docTypeParts, htmlVersion);
                default: return GetTransitionalDocType(htmlVersion);
            }
        }

        private static string GetRdfaDocType()
        {
            return @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML+RDFa 1.0//EN"" ""http://www.w3.org/MarkUp/DTD/xhtml-rdfa-1.dtd"">";
        }

        private static string GetHtml5DocType()
        {
            return @"<!DOCTYPE html>";
        }

        private static string GetFramesetDocType(HtmlVersion htmlVersion)
        {
            switch (htmlVersion)
            {
                case HtmlVersion.Html4:
                    return @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Frameset//EN"" ""http://www.w3.org/TR/html4/frameset.dtd"">";
                default:
                    return @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Frameset//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd"">";
            }
        }

        private static string GetStrictDocType(HtmlVersion htmlVersion)
        {
            switch (htmlVersion)
            {
                case HtmlVersion.Html4:
                    return @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">";
                default:
                    return @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">";
            }
        }

        private static string GetBasicDocType()
        {
            return @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML Basic 1.1//EN"" ""http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd"">";
        }

        private static string GetMobileDocType()
        {
            return @"<!DOCTYPE html PUBLIC ""-//WAPFORUM//DTD XHTML Mobile 1.2//EN"" ""http://www.openmobilealliance.org/tech/DTD/xhtml-mobile12.dtd"">";
        }

        private static string GetXHtmlDocType11()
        {
            return @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">";
        }

        private static string GetXmlDocType(IList<string> docTypeParts, HtmlVersion htmlVersion)
        {
            if (htmlVersion != HtmlVersion.XHtml) return "";

            string encoding = docTypeParts.Count < 2
                ? "utf-8"
                : docTypeParts[1];
            return "<?xml version='1.0' encoding='" + encoding + "' ?>";
        }

        private static string GetTransitionalDocType(HtmlVersion htmlVersion)
        {
            switch (htmlVersion)
            {
                case HtmlVersion.Html4:
                    return @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">";
                default:
                    return @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">";
            }
        }
    }
}
