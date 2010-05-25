using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Core.Ast;
using NHaml.Core.Template;

namespace NHaml.Core.Visitors
{
    public static class MetaDataFiller
    {
        /// <summary>
        /// Fills the page/control/master metadata using default values and returns the node that contains the main metadata
        /// </summary>
        /// <param name="metadata">The metadata container of the document</param>
        /// <returns>The page declaration metadata container</returns>
        public static MetaNode FillAndGetPageDefinition(Dictionary<string, List<MetaNode>> metadata, TemplateOptions options)
        {
            MetaNode pagedefiniton = null;
            List<MetaNode> data;

            if (metadata.ContainsKey("contentplaceholder"))
            {
                pagedefiniton = new MetaNode("master");
            }
            else
            {
                pagedefiniton = new MetaNode("page");
            }

            if (metadata.TryGetValue("page", out data))
            {
                pagedefiniton = data[0];
            }
            if (metadata.TryGetValue("control", out data))
            {
                pagedefiniton = data[0];
            }
            if (metadata.TryGetValue("master", out data))
            {
                pagedefiniton = data[0];
            }

            if (pagedefiniton.Attributes.Find(x => x.Name == "Language") == null)
            {
                pagedefiniton.Attributes.Add(new AttributeNode("Language") { Value = new TextNode(new TextChunk("C#")) });
            }

            if (pagedefiniton.Attributes.Find(x => x.Name == "AutoEventWireup") == null)
            {
                pagedefiniton.Attributes.Add(new AttributeNode("AutoEventWireup") { Value = new TextNode(new TextChunk("true")) });
            }

            if (pagedefiniton.Attributes.Find(x => x.Name == "Inherits") == null)
            {
                pagedefiniton.Attributes.Add(new AttributeNode("Inherits") { Value = new TextNode(new TextChunk(options.TemplateBaseType.Name)) });
            }

            if (metadata.TryGetValue("type", out data))
            {
                var tc = pagedefiniton.Attributes.Find(x => x.Name == "Inherits");
                tc.Value = new TextNode(new TextChunk(((tc.Value as TextNode).Chunks[0] as TextChunk).Text + "<" + data[0].Value + ">"));
            }

            if (!metadata.ContainsKey(pagedefiniton.Name))
            {
                metadata[pagedefiniton.Name] = new List<MetaNode> { pagedefiniton };
            }

            return pagedefiniton;
        }
    }
}
