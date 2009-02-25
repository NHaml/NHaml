using System;
using System.Windows;
using System.Windows.Documents;

namespace NHaml.Xps
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DocumentDialog : Window
    {
        private object document;
        public DocumentDialog()
        {
            InitializeComponent();
        }
        public void LoadDocument(object document)
        {
            this.document = document;
            if (document is FlowDocument)
            {
                documentViewer.Visibility = Visibility.Hidden;
                flowDocumentReader.Document = (FlowDocument)document;

            }
            else if (document is IDocumentPaginatorSource)
            {
                flowDocumentReader.Visibility = Visibility.Hidden;
                documentViewer.Document = (IDocumentPaginatorSource)document;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            if (document is FlowDocument)
            {
                documentViewer.Print();
            }
            else if (document is FixedDocument || document is FixedDocumentSequence)
            {
                flowDocumentReader.Print();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}