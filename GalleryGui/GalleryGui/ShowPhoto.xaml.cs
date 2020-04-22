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
        private string _userId;
        private GalleryApi api;

        public ShowPhoto(Img photo, string userId)
        {
            InitializeComponent();

            this._userId = userId;
            api = new GalleryApi();

            displayImage(photo);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void displayImage(Img photo)
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
                
                List<string> usersList = api.getTaggedUsers(photo);
                int i = 0;
                for (i = 0; i < usersList.Count() - 1; i++)
                {
                    UsersTags.Text += usersList[i];

                    // Add new line after 4 names in a row.
                    if ((i + 1) % 4 == 0)
                    {
                        UsersTags.Text += System.Environment.NewLine;
                    }
                    else
                    {
                        UsersTags.Text += ", ";
                    }
                }
                if (usersList.Count() > 0)
                    UsersTags.Text += usersList[i];

                TagsAmount.Text += usersList.Count();

                // If photo is not of user hide delete button.
                if (_userId != api.getPhotoOwner(photo).ToString())
                {
                    DeletePhoto.Visibility = System.Windows.Visibility.Hidden;
                }
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

        private void deleteBTN_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
