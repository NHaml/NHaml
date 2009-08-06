using System;
using System.ComponentModel;
using System.IO;
using System.Printing;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Xml;

namespace NHaml.Xps
{
    public class XpsEngine
    {
        public XpsEngine()
        {
            TemplateEngine = new TemplateEngine();
            TemplateEngine.Options.AddReferences(typeof(DataView<>));
        }

        static XpsEngine()
        {
            Instance = new XpsEngine();
        }

        public static XpsEngine Instance { get; private set; }

        public TemplateEngine TemplateEngine { get; private set; }



        public void Generate<TData>(string viewPath, TData context, string targetPath)
        {
            var load = GetLoad(viewPath, context);
            using (var document = new XpsDocument(targetPath, FileAccess.Write))
            {
                var xpsDocumentWriter = XpsDocument.CreateXpsDocumentWriter(document);
                WriteLoadToXpsDocumentWriter(load, xpsDocumentWriter);
                document.Close();
            }
        }


        public object GetLoad<TData>(string viewPath, TData data)
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


        public void Print<TData>(string viewPath, TData context, System.Func<PrintQueue> getPrintQueue, AsyncCompletedEventHandler callback)
        {
            var load = GetLoad(viewPath, context);
            WriteLoadToXpsDocumentWriter(load, getPrintQueue, callback);
        }

        public void PrintPreview<TData>(string viewPath, TData context)
        {
            var load = GetLoad(viewPath, context);
            var documentViewHostDialog = new DocumentDialog();
            documentViewHostDialog.LoadDocument(load);
            documentViewHostDialog.ShowDialog();
        }

        private void WriteLoadToXpsDocumentWriter(object load, System.Func<PrintQueue> getPrintQueue, AsyncCompletedEventHandler callback)
        {

            var thread = new Thread(delegate(object obj)
                                        {
                                            AsyncCompletedEventArgs eventArgs;
                                            try
                                            {
                                                using (var printQueue = getPrintQueue())
                                                {
                                                    var xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);

                                                    WriteLoadToXpsDocumentWriter(load, xpsDocumentWriter);
                                                }
                                                eventArgs = new AsyncCompletedEventArgs(null, false, null);
                                            }
                                            catch (Exception exception)
                                            {
                                                eventArgs = new AsyncCompletedEventArgs(exception, false, null);
                                            }
                                            if (callback != null)
                                            {
                                                callback(this, eventArgs);
                                            }
                                        });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private static void WriteLoadToXpsDocumentWriter(object load, XpsDocumentWriter xpsDocumentWriter)
        {
            var documentPaginatorSource = load as IDocumentPaginatorSource;
            if (documentPaginatorSource != null)
            {
                xpsDocumentWriter.Write(documentPaginatorSource.DocumentPaginator);
                return;
            }
            var visual = load as Visual;
            if (visual == null)
            {
                xpsDocumentWriter.Write(visual, new PrintTicket());
                return;
            }
        }
    }
}