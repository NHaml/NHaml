using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Printing;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using NHaml.TemplateResolution;

namespace NHaml.Xps
{
    public class XpsEngine
    {
        public XpsEngine(TemplateEngine templateEngine)
        {
            TemplateEngine = new TemplateEngine();
            TemplateEngine.Options.AddReferences(typeof(DataView<>));
        }

        public XpsEngine():this(new TemplateEngine())
        {
        }


        public TemplateEngine TemplateEngine { get; set; }

        public virtual void Generate<TData>(IList<IViewSource> viewSources, TData data, string targetPath)
        {
            DependencyObject dependencyObject;
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                Render(viewSources, streamWriter, data);
                streamWriter.Flush();
                memoryStream.Position = 0;
                dependencyObject = Load(memoryStream);
            }
            using (var document = new XpsDocument(targetPath, FileAccess.Write))
            {
                var xpsDocumentWriter = XpsDocument.CreateXpsDocumentWriter(document);
                WriteLoadToXpsDocumentWriter(dependencyObject, xpsDocumentWriter, null);
            }
        }

        private static DependencyObject Load(Stream memoryStream)
        {
            return (DependencyObject)XamlReader.Load(memoryStream);
        }


        private void Render<TData>(IList<IViewSource> viewSources, TextWriter textWriter, TData data)
        {
            var view = (DataView<TData>)TemplateEngine.Compile(viewSources, typeof(DataView<TData>)).CreateInstance();
            view.ViewData = data;
            view.Render(textWriter);
        }


        public virtual void Print<TData>(IList<IViewSource> viewSources, TData data, string localPrintQueueName, PrintTicket printTicket, AsyncCompletedEventHandler callback)
        {
            Print(viewSources, data, () => GetNamedLocalPrintQueue(localPrintQueueName), printTicket, callback);
        }

        private static PrintQueue GetNamedLocalPrintQueue(string localPrintQueueName)
        {
            using (var printServer = new LocalPrintServer())
            {
                return printServer.GetPrintQueue(localPrintQueueName);
            }
        }

        public virtual void Print<TData>(IList<IViewSource> viewSources, TData data, System.Func<PrintQueue> getPrintQueue, PrintTicket printTicket, AsyncCompletedEventHandler callback)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            Render(viewSources, streamWriter, data);
            streamWriter.Flush();
            memoryStream.Position = 0;
            WriteLoadToXpsDocumentWriter(memoryStream, getPrintQueue, printTicket, callback);
        }

        public virtual void PrintPreview<TData>(IList<IViewSource> viewSources, TData data)
        {
            DependencyObject dependencyObject;
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                Render(viewSources, streamWriter, data);
                streamWriter.Flush();
                memoryStream.Position = 0;
                dependencyObject = Load(memoryStream);
            }
            var documentViewHostDialog = new DocumentDialog();
            documentViewHostDialog.LoadDocument(dependencyObject);
            documentViewHostDialog.ShowDialog();
        }

        private void WriteLoadToXpsDocumentWriter(Stream render, System.Func<PrintQueue> getPrintQueue, PrintTicket printTicket, AsyncCompletedEventHandler callback)
        {
            var thread = new Thread(delegate(object obj)
                                    {
                                        AsyncCompletedEventArgs eventArgs;
                                        try
                                        {
                                            var load = Load(render);
                                            GetQueueAndPrint(getPrintQueue, load, printTicket);
                                           
                                            eventArgs = new AsyncCompletedEventArgs(null, false, null);
                                        }
                                        catch (Exception exception)
                                        {
                                            eventArgs = new AsyncCompletedEventArgs(exception, false, null);
                                        }
                                        finally
                                        {
                                            render.Close();
                                        }
                                        if (callback != null)
                                        {
                                            callback(this, eventArgs);
                                        }
                                    });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        protected virtual void GetQueueAndPrint(System.Func<PrintQueue> getPrintQueue, DependencyObject load, PrintTicket printTicket)
        {
            using (var printQueue = getPrintQueue())
            {
                var xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);
                WriteLoadToXpsDocumentWriter(load, xpsDocumentWriter, printTicket);
            }
        }

        protected virtual void WriteLoadToXpsDocumentWriter(DependencyObject dependencyObject, XpsDocumentWriter xpsDocumentWriter, PrintTicket printTicket)
        {
            var documentPaginatorSource = dependencyObject as IDocumentPaginatorSource;
            if (documentPaginatorSource != null)
            {
                xpsDocumentWriter.Write(documentPaginatorSource.DocumentPaginator, printTicket);
                return;
            }
            var visual = dependencyObject as Visual;
            if (visual == null)
            {
                xpsDocumentWriter.Write(visual, printTicket);
                return;
            }
        }
    }
}