using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ImageRecognitionML.Model;
using MessageBox = System.Windows.MessageBox;

namespace ImageRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int _imageIndex = 0;
        private string _selectedPrediction;
        private readonly List<ImageDetails> _imagesSources = new List<ImageDetails>();
        private readonly List<string> _predictions = new List<string>
        {
            "Human",
            "Mountains"
        };


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImagesList.Items.Clear();
            if (_selectedPrediction == null)
            {
                MessageBox.Show("Select your prediction, before you pick folder.");
                return;
            }
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            _imageIndex = 0;
            _imagesSources.Clear();

            var directory = new DirectoryInfo(dialog.SelectedPath);
            var files = directory.GetFiles();

            var input = new ModelInput();
            var index = 0;

            foreach(var file in files.Where(x => ".jpg|.jpeg|.png".Contains(x.Extension.ToLower())))
            {
                input.ImageSource = file.FullName;
                var result = ConsumeModel.Predict(input);
                if(!string.Equals(result.Prediction, _selectedPrediction, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                var accuracy = result.Score.Max();
                if (accuracy < 0.8)
                    continue;
                _imagesSources.Add(new ImageDetails(file.Name, file.FullName, index));
                index++;
            }
            _imagesSources.ForEach(x => ImagesList.Items.Add(x));
            ImagesList.DisplayMemberPath = "Name";
            LoadImage();
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
            LoadImage();
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
            LoadImage();
        }

        private void LoadImage()
        {
            var uri = !_imagesSources.Any()
                ? new Uri("C:\\Users\\Dawid\\RiderProjects\\image-recognition\\ImageRecognition\\Files\\404_file_not_found.jpg", UriKind.Absolute)
                : new Uri(_imagesSources[_imageIndex].FullName, UriKind.Absolute);
            var source = new BitmapImage();
            source.BeginInit();
            source.UriSource = uri;
            source.EndInit();
            MainImage.Source = source;
        }

        private void ImagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.AddedItems[0] is ImageDetails imageDetails))
                return;
            _imageIndex = imageDetails.Index;
            LoadImage();

        }

        private void Predictions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.AddedItems[0] is ListBoxItem prediction))
                return;
            _selectedPrediction = prediction.Content.ToString();
        }
    }
}
