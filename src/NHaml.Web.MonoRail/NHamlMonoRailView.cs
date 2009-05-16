using System;
using System.Collections;
using System.IO;
using System.Security.Permissions;
using System.Web;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using NHaml.Utils;

namespace NHaml.Web.MonoRail
{
    //TODO: the monorail integration here is based mostly on the brail view engine. Need to talk to castle guys and ask them how they want me to document this fact.

    [AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    [AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    public abstract class NHamlMonoRailView : Template
    {
        private NHamlMonoRailView parent;

        public void Render(IEngineContext engineContext, TextWriter writer, IControllerContext controllerContext)
        {
            Invariant.ArgumentNotNull(controllerContext, "controllerContext");
            Invariant.ArgumentNotNull(engineContext, "engineContext");

            ViewContext = engineContext;

            ViewData = controllerContext.PropertyBag;

            CreateHelpers(engineContext);

            Render(writer);
        }

        /// <summary>
        /// This is required because we may want to replace the output stream and get the correct
        /// behavior from components call RenderText() or RenderSection()
        /// </summary>
        public IDisposable SetOutputStream(TextWriter newOutputStream)
        {
            var disposable = new ReturnOutputStreamToInitialWriter(Output.TextWriter, this);
            Output.TextWriter = newOutputStream;
            return disposable;
        }

        protected virtual void CreateHelpers(IEngineContext engineContext)
        {
            Ajax = new AjaxHelper(engineContext);
            Html = new HtmlHelper(engineContext);
            Url = new UrlHelper(engineContext);
        }

        public AjaxHelper Ajax { get; protected set; }
        public HtmlHelper Html { get; protected set; }
        public UrlHelper Url { get; protected set; }

        public IEngineContext ViewContext { get; private set; }

        public IDictionary ViewData { get; private set; }

        public NHamlMonoRailViewEngine ViewEngine { get; set; }



        public void Component(string componentName)
        {
            Component(componentName, new Hashtable());
        }

        public void Component(string componentName, IDictionary parameters)
        {
			var componentContext =
                new NHamlViewComponentContext(this, null, componentName, Output.TextWriter,
				                              new Hashtable(parameters, StringComparer.InvariantCultureIgnoreCase));
			AddViewComponentProperties(componentContext.ComponentParameters);
			var componentFactory = (IViewComponentFactory) ViewContext.GetService(typeof(IViewComponentFactory));
			var component = componentFactory.Create(componentName);
			component.Init(ViewContext, componentContext);
			component.Render();
			if (componentContext.ViewToRender != null)
			{
				OutputSubView("/" + componentContext.ViewToRender, componentContext.ComponentParameters);
			}
			RemoveViewComponentProperties(componentContext.ComponentParameters);
		}
        /// <summary>
        /// Adds the view component newProperties.
        /// This will be included in the parameters searching, note that this override
        /// the current parameters if there are clashing.
        /// The search order is LIFO
        /// </summary>
        /// <param name="newProperties">The newProperties.</param>
        public void AddViewComponentProperties(IDictionary newProperties)
        {
            if (viewComponentsParameters == null)
                viewComponentsParameters = new ArrayList();
            viewComponentsParameters.Insert(0, newProperties);
        }

        /// <summary>
        /// Removes the view component properties, so they will no longer be visible to the views.
        /// </summary>
        /// <param name="propertiesToRemove">The properties to remove.</param>
        public void RemoveViewComponentProperties(IDictionary propertiesToRemove)
        {
            if (viewComponentsParameters == null)
                return;
            viewComponentsParameters.Remove(propertiesToRemove);
        }
        /// <summary>
        /// used to hold the ComponentParams from the view, so their views/sections could access them
        /// </summary>
        private IList viewComponentsParameters;
        private class ReturnOutputStreamToInitialWriter : IDisposable
        {
            private readonly TextWriter initialWriter;
            private NHamlMonoRailView parent;

            public ReturnOutputStreamToInitialWriter(TextWriter initialWriter, NHamlMonoRailView parent)
            {
                this.initialWriter = initialWriter;
                this.parent = parent;
            }

            public void Dispose()
            {
                parent.Output.TextWriter = initialWriter;
            }

        }

        /// <summary>
        /// Output the subview to the client, this is either a relative path "SubView" which
        /// is relative to the current /script/ or an "absolute" path "/home/menu" which is
        /// actually relative to ViewDirRoot
        /// </summary>
        /// <param name="subviewName"></param>
        public string OutputSubView(string subviewName)
        {
            return OutputSubView(subviewName, new Hashtable());
        }

        /// <summary>
        /// Similiar to the OutputSubView(string) function, but with a bunch of parameters that are used
        /// just for this subview. This parameters are /not/ inheritable.
        /// </summary>
        /// <returns>An empty string, just to make it possible to use inline ${OutputSubView("foo")}</returns>
        public string OutputSubView(string subviewName, IDictionary parameters)
        {
            OutputSubView(subviewName, Output.TextWriter, parameters);
            return string.Empty;
        }

        /// <summary>
        /// Outputs the sub view to the writer
        /// </summary>
        /// <param name="subviewName">Name of the subview.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="parameters">The parameters.</param>
        public void OutputSubView(string subviewName, TextWriter writer, IDictionary parameters)
        {
            var subViewFileName = GetSubViewFilename(subviewName);
            var subView = ViewEngine.GetCompiledScriptInstance(subViewFileName);
            subView.SetParent(this);
            foreach (DictionaryEntry entry in parameters)
            {
                subView.ViewData[entry.Key] = entry.Value;
            }
            subView.Render(ViewContext, writer, ViewContext.CurrentControllerContext);
            foreach (DictionaryEntry entry in subView.ViewData)
            {
                if (subView.ViewData.Contains(entry.Key + ".@bubbleUp") == false)
                {
                    continue;
                }
                ViewData[entry.Key] = entry.Value;
                ViewData[entry.Key + ".@bubbleUp"] = true;
            }
        }

        private void SetParent(NHamlMonoRailView view)
        {
            parent = view;
        }

        /// <summary>
        /// Get the sub view file name, if the subview starts with a '/' 
        /// then the filename is considered relative to ViewDirRoot
        /// otherwise, it's relative to the current script directory
        /// </summary>
        /// <param name="subviewName"></param>
        /// <returns></returns>
        public string GetSubViewFilename(string subviewName)
        {
            //absolute path from Views directory
            if (subviewName[0] == '/')
                return subviewName.Substring(1) + ViewEngine.ViewFileExtension;
            return Path.Combine(ViewEngine.ViewRootDir, subviewName) + ViewEngine.ViewFileExtension;
        }
    }
}