using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageRecognitionML.Model;

namespace ImageRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int _imageIndex = 0;
        private readonly List<string> _imagesSources = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            _imageIndex = 0;
            _imagesSources.Clear();

            var directory = new DirectoryInfo(dialog.SelectedPath);
            var files = directory.GetFiles();

            var input = new ModelInput();

            foreach(var file in files.Where(x => ".jpg|.jpeg|.png".Contains(x.Extension.ToLower())))
            {
                input.ImageSource = file.FullName;
                var result = ConsumeModel.Predict(input);
                var accuracy = result.Score.Max();
                if (accuracy < 0.8)
                    continue;
                //MessageBox.Show($"You selected: {file.Name} | Prediction: {result.Prediction} | Accuracy: {accuracy}");
                _imagesSources.Add(file.FullName);
            }

            GetImage();
        }

        private void PreviousBTN_Click(object sender, RoutedEventArgs e)
        {
            _imageIndex--;
            if (_imageIndex < 0)
            {
                _imageIndex = 0;
                return;
            }

            Console.WriteLine($"ImageIndex: {_imageIndex} | ImagesCount: {_imagesSources.Count} ");
            GetImage();
        }

        private void NextBTN_Click(object sender, RoutedEventArgs e)
        {
            _imageIndex++;

            if (_imageIndex > _imagesSources.Count - 1)
            {
                _imageIndex = _imagesSources.Count - 1;
                return;
            }

            Console.WriteLine($"ImageIndex: {_imageIndex} | ImagesCount: {_imagesSources.Count} ");
            GetImage();
        }

        private void GetImage()
        {
            var uri = new Uri(_imagesSources[_imageIndex], UriKind.Absolute);
            var source = new BitmapImage();
            source.BeginInit();
            source.UriSource = uri;
            source.EndInit();
            MainImage.Source = source;
        }
    }
}
