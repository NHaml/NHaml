using System;
using System.Collections;
using System.IO;
using Castle.MonoRail.Framework;

namespace NHaml.Web.MonoRail
{
    public class NHamlViewComponentContext : IViewComponentContext
    {
        private NHamlMonoRailView parent;
        private IDictionary sections;

        /// <summary>
        /// Initializes a new instance of the <see cref="NHamlViewComponentContext"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="body">The body.</param>
        /// <param name="name">The name.</param>
        /// <param name="text">The text.</param>
        /// <param name="parameters">The parameters.</param>
        public NHamlViewComponentContext(NHamlMonoRailView parent, Action<TextWriter> body, string name, TextWriter text, IDictionary parameters)
        {
            this.parent = parent;
            Body = body;
            ComponentName = name;
            Writer = text;
            ComponentParameters = parameters;
        }


        public Action<TextWriter> Body { get; set; }


        public string ComponentName { get; private set; }

        public IDictionary ContextVars
        {
            get { return parent.PropertyBag; }
        }

        public IDictionary ComponentParameters { get; private set; }

        public string ViewToRender { get; set; }

        public TextWriter Writer { get; private set; }

        public void RenderBody()
        {
            RenderBody(Writer);
        }

        public void RenderBody(TextWriter writer)
        {
            if (Body == null)
            {
                throw new MonoRailException("This component does not have a body content to be rendered");
            }
            using (parent.SetOutputStream(writer))
            {
                Body(writer);
            }
        }

        /// <summary>
        /// Pendent
        /// </summary>
        /// <param name="name"></param>
        /// <param name="writer"></param>
        public void RenderView(string name, TextWriter writer)
        {
            parent.OutputSubView(name, writer, ContextVars);
        }

        public bool HasSection(string sectionName)
        {
            return sections != null && sections.Contains(sectionName);
        }

        public void RenderSection(string sectionName)
        {
            RenderSection(sectionName, Writer);
        }

        /// <summary>
        /// Renders the the specified section
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="writer">The writer.</param>
        public void RenderSection(string sectionName, TextWriter writer)
        {
            if (HasSection(sectionName) == false)
            {
                return; //matching the NVelocity behavior, but maybe should throw?
            }
            var callable = (Action<TextWriter>) sections[sectionName];
            callable(writer );
        }

        public IViewEngine ViewEngine
        {
            get
            {
                return parent.ViewEngine;
            }
        }


        public void RegisterSection(string name, Action<TextWriter> section)
        {
            if (sections == null)
            {
                sections = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            }
            sections[name] = section;
        }
    }
}