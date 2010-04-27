using System;
using System.IO;
using System.Text;
using System.Windows;

namespace NHaml.Generator.Wpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Convert(object sender, RoutedEventArgs e)
        {
            try
            {
                nhamlTextBox.Clear();
                var importer = new Generator {IncludeDocType = true};
                var provider = new StringBuilder();
                using (var reader = new StringReader(htmlTextBox.Text))
                {
                    using (var writer = new StringWriter(provider))
                    {
                        importer.Import(reader,writer);
            
                        nhamlTextBox.Text = provider.ToString();
                    }
                }

            }
            catch (Exception exception)
            {
                errorsTextBox.Text = exception.ToString();
            }
        }
    }
}