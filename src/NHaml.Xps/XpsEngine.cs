using System;
using System.IO;
using System.Printing;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using System.Xml;

namespace NHaml.Xps
{
    public class XpsEngine
    {
        static XpsEngine instance;
        public XpsEngine()
        {
            TemplateEngine = new TemplateEngine();
            TemplateEngine.AddReferences(typeof(DataView<>));
        }

        static XpsEngine()
        {
            instance = new XpsEngine();
        }
        public static XpsEngine Instance
        {
            get
            {
                return instance;
            }
        }
        public TemplateEngine TemplateEngine { get; private set; }


        public void Generate<TData>(string viewPath, TData context, string targetPath) where TData : class
        {
            var load = GetLoad(viewPath, context);
            using (var document = new XpsDocument(targetPath, FileAccess.Write))
            {
                var xpsDocumentWriter = XpsDocument.CreateXpsDocumentWriter(document);
                WriteLoadToXpsDocumentWriter(load, xpsDocumentWriter);
                document.Close();
            }
        }

        public void GenerateAsync<TData>(string viewPath, TData context, string targetPath, AsyncCallback asyncCallback) where TData : class
        {
            ThreadStart thread = () => Generate(viewPath, context, targetPath);
            thread.BeginInvoke(asyncCallback, targetPath);
        }

        private object GetLoad<TData>(string viewPath, TData data) where TData : class
        {
            var view = (DataView<TData>)TemplateEngine.Compile(viewPath, typeof(DataView<TData>)).CreateInstance();
            view.ViewData = data;
            var stringBuilder = new StringBuilder();
            using (TextWriter textWriter = new StringWriter(stringBuilder))
            {
                view.Render(textWriter);
            }
            var render = stringBuilder.ToString();
            using (var stringReader = new StringReader(render))
            {
                using (var reader = new XmlTextReader(stringReader))
                {
                    try
                    {
                        return XamlReader.Load(reader);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(string.Format("Could not parse XML. {0}{1}", Environment.NewLine, render), exception);
                    }
                }
            }
        }

        public void Print<TData>(string viewPath, TData context) where TData : class
        {
            using (var queue = LocalPrintServer.GetDefaultPrintQueue())
            {
                Print(viewPath, context, queue);
            }
        }

        public void Print<TData>(string viewPath, TData context, PrintQueue queue) where TData : class
        {
            var load = GetLoad(viewPath, context);
            var xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(queue);
            WriteLoadToXpsDocumentWriter(load, xpsDocumentWriter);
        }

        public void PrintAsync<TData>(string viewPath, TData context) where TData : class
        {
            var thread = new Thread(
                () => Print(viewPath, context));
            thread.Start();
        }

        public void PrintAsync<TData>(string viewPath, TData context, Func<PrintQueue> getQueue, AsyncCallback asyncCallback) where TData : class
        {
            ParameterizedThreadStart thread2 = delegate(object obj)
                                                   {
                                                       var getQueue1 = (Func<PrintQueue>) obj;
                                                       using (var printQueue = getQueue1())
                                                       {
                                                           Print(viewPath, context, printQueue);
                                                       }
                                                   };
            thread2.BeginInvoke(getQueue, asyncCallback, null);
        }

        public void PrintPreview<TData>(string viewPath, TData context) where TData : class
        {
            var load = GetLoad(viewPath, context);
            var documentViewHostDialog = new DocumentDialog();
            documentViewHostDialog.LoadDocument(load);
            documentViewHostDialog.ShowDialog();
        }

        private static void WriteLoadToXpsDocumentWriter(object load, SerializerWriter xpsDocumentWriter)
        {
            var documentPaginatorSource = load as IDocumentPaginatorSource;
            if (documentPaginatorSource != null)
            {
                xpsDocumentWriter.Write(documentPaginatorSource.DocumentPaginator);
            }
            else
            {
                var visual = load as Visual;
                if (visual == null)
                {
                    throw new NotImplementedException();
                }
                xpsDocumentWriter.Write(visual);
            }
        }
    }
}