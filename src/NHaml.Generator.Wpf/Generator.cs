using System;
using System.IO;
using System.Text;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace NHaml.Generator.Wpf
{
    public class Generator
    {
        private int indent;
        TextWriter textWriter;
        private bool isFirstElement = true;
        private bool isInsidePre;
        public string IndentString{ get; set;}


        public bool IncludeDocType { get; set; }

        public Generator()
        {
            IndentString = "  ";
            IncludeDocType = true;
        }

        public void Import(TextReader textReader, TextWriter textWriter)
        {
            this.textWriter = textWriter;
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(textReader);
            var navigator = htmlDocument.CreateNavigator();
            navigator.MoveToRoot();
            RecursiveWalkThroughXpath(navigator);
            textWriter.Flush();
        }
        public void RecursiveWalkThroughXpath(XPathNavigator navigator)
        {
            switch (navigator.NodeType)
            {
                case XPathNodeType.Root:
                    //TODO: do this better ie parse xml or html decelration
                    if (IncludeDocType)
                    {
                        textWriter.Write("!!!");
                        textWriter.Write(Environment.NewLine);
                    }
                    break;
                case XPathNodeType.Element:
                    ProcessElement(navigator);
                    break;
                case XPathNodeType.Text:
                    ProcessText(navigator);

                    break;
            }

            if (navigator.MoveToFirstChild())
            {
                do
                {
                    RecursiveWalkThroughXpath(navigator);
                } while (navigator.MoveToNext());

                navigator.MoveToParent();
                CheckUnIndent(navigator);
            }
            else
            {
                CheckUnIndent(navigator);
            }
        }

        private void CheckUnIndent(XPathNavigator navigator)
        {
            if (navigator.NodeType == XPathNodeType.Element)
            {
                indent--;
                if (navigator.LocalName.ToLower() == "pre")
                {
                    isInsidePre = false;
                }
            }
        }

        private void ProcessText(XPathNavigator navigator)
        {
            var value = navigator.Value;
            if (isInsidePre)
            {
                var split = value.Split(new[]{Environment.NewLine}, StringSplitOptions.None);
                for (var index = 0; index < split.Length; index++)
                {
                    var line = split[index];
                    line = TrimWhiteSpace(line);
                    if (index!=0 && line.Length>0)
                    {
                        WriteIndent();
                    }
                    if (index < split.Length - 1 || line.Length > 0)
                    {
                        textWriter.WriteLine(line);
                    }
                }
            }
            else
            {
                value = TrimWhiteSpace(value);
                if (value.Length > 0)
                {
                    textWriter.Write(" " + value);
                }
            }
        }

        private static string TrimWhiteSpace(string value)
        {
            while (true)
            {
                if (value.StartsWith(" "))
                {
                    value = value.Substring(1, value.Length - 1);
                    continue;
                }
                if (value.StartsWith("\t"))
                {
                    value = value.Substring(1, value.Length - 1);
                    continue;
                }
				if (value.StartsWith(Environment.NewLine))
                {
                    value = value.Substring(2, value.Length - 2);
                    continue;
                }
                if (value.EndsWith(" "))
                {
                    value = value.Substring(0, value.Length - 1);
                    continue;
                }
                if (value.EndsWith("\t"))
                {
                    value = value.Substring(0, value.Length - 1);
                    continue;
                }
				if (value.EndsWith(Environment.NewLine))
                {
                    value = value.Substring(0, value.Length - 2);
                    continue;
                }
                break;
            }

        
            return value;
        }


        private void ProcessElement(XPathNavigator navigator)
        {
            //TODO: prob a better way of doing this using the navigator
            if (!isFirstElement)
            {
                textWriter.Write(Environment.NewLine);
            }
            isFirstElement = false;
            WriteIndent();
            if (navigator.LocalName.ToLower() == "pre")
            {
                isInsidePre = true;
            }
            if (navigator.LocalName == "div")
            {
                var temp = navigator.CreateNavigator();
                if (temp.MoveToAttribute("id", null))
                {
                    textWriter.Write("#");
                    textWriter.Write(temp.Value);
                }
                else if (temp.MoveToAttribute("class", null))
                {
                    var strings = temp.Value.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in strings)
                    {
                        textWriter.Write(".");
                        textWriter.Write(s);
                    }
                }
                else
                {
                    textWriter.Write("%div");       
                }
            }
            else if (navigator.Prefix == string.Empty)
            {
                textWriter.Write("%{0}", navigator.LocalName);
                CheckForAndAppendClass(navigator);
            }
            //else
            //{
                //TODO:
                //Console.Write(" <{0}:{1}>",oNavigator.Prefix,oNavigator.LocalName);
                //Console.WriteLine("\t" + oNavigator.NamespaceURI);
            //}

            var attributeNavigator = navigator.CreateNavigator();
            ProcessAttributes(attributeNavigator);
         
            indent++;
        }

        private void CheckForAndAppendClass(IXPathNavigable navigator)
        {
            var temp = navigator.CreateNavigator();
            if (temp.MoveToAttribute("class", null))
            {
                textWriter.Write(".");
                textWriter.Write(temp.Value);
            }
        }

        private void ProcessAttributes(XPathNavigator attributeNavigator)
        {
            var stringBuilder = new StringBuilder();
            var added = false;
            while (attributeNavigator.MoveToNextAttribute())
            {
                if ((attributeNavigator.LocalName != "id") && (attributeNavigator.LocalName != "class"))
                {
                    added = true;
                    stringBuilder.AppendFormat("{0}=\"{1}\" ", attributeNavigator.LocalName, attributeNavigator.Value);
                }
            }
            if (added)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            if (stringBuilder.Length > 0)
            {
                textWriter.Write("{");
                textWriter.Write(stringBuilder);
                textWriter.Write("}");
            }
        }

        private void WriteIndent()
        {
            for (var i = 0; i < indent; i++)
            {
                textWriter.Write(IndentString);
            }
        }
    }
}