using System.IO;
using System.Windows;
using System.Windows.Forms;
using ImageRecognitionML.Model;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace ImageRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var directory = new DirectoryInfo(dialog.SelectedPath);
            var files = directory.GetFiles();

            var input = new ModelInput();

            foreach(var file in files)
            {
                input.ImageSource = file.FullName;
                var result = ConsumeModel.Predict(input);

                MessageBox.Show("You selected: " + result.Prediction);
            }
        }
    }
}
