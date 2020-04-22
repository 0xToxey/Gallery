using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GalleryGui
{
    /// <summary>
    /// Interaction logic for ShowPhoto.xaml
    /// </summary>
    public partial class ShowPhoto : Window
    {
        public ShowPhoto(img photo)
        {
            InitializeComponent();

            displayImage(photo);
        }

        private void displayImage(img photo)
        {
            try
            {
                var uriSource = new Uri(photo.Path, UriKind.Absolute);
                ImageNameLabel.Content = photo.name;
                Image.Source = new BitmapImage(uriSource);

                IdText.Text += photo.ID;
                CreationDate.Text += photo.date;
                AlbumID.Text += photo.albumID;
                photoName.Text += photo.name;
            }
            catch
            {
                throw;
            }
        }

        private void closeBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
