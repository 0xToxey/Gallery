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
    /// Interaction logic for ShowAlbum.xaml
    /// </summary>
    public partial class ShowAlbum : Window
    {
        private string _userId;
        private Album opendAlbum;
        private GalleryApi api;

        public ShowAlbum(Album album, string userId)
        {
            InitializeComponent();

            this._userId = userId;
            api = new GalleryApi();
            this.opendAlbum = album;

            loadAlbumDetails();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void closeBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void deleteBTN_Click(object sender, RoutedEventArgs e)
        {
            api.deleteAlbum(this.opendAlbum);
            this.Close();
        }

        public void loadAlbumDetails()
        {
            // If the album is not of user dont show delete button.
            if (opendAlbum.ownerID.ToString() != _userId)
            {
                ChangeAlbum.Visibility = System.Windows.Visibility.Hidden;
            }

            // Display details
            AlbumNameDisplay.Content = opendAlbum.name;
            IdText.Text = "Id: " + opendAlbum.ID.ToString();
            CreationDate.Text = "Date: " + opendAlbum.date;
            OwnerID.Text = "Owner Id: " + opendAlbum.ownerID.ToString();
            albumName.Text = "Name: " + opendAlbum.name;

            PicAmount.Text = "Photos amount: " + api.getAlbumPhotos(this.opendAlbum).Count().ToString();
            TagsAmount.Text = "Tags amount: " + api.albumTagsAmount(this.opendAlbum).ToString();

            loadPhotos();
        }

        private void loadPhotos()
        {
            List<Img> imgList = api.getAlbumPhotos(this.opendAlbum);
            photosList.Items.Clear();

            foreach (Img img in imgList)
            {
                List<Img> tempImg = new List<Img>();
                tempImg.Add(img);

                photosList.Items.Add(tempImg);
            }
            photosList.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(PhotoOpenClick), true);
        }

        private void PhotoOpenClick(object sender, RoutedEventArgs e)
        {
            Img item = ((sender as ListBox).SelectedItem as List<Img>)[0];
            if (item != null)
            {
                try
                {
                    ShowPhoto photoOpen = new ShowPhoto(item, _userId);
                    photoOpen.Show();
                }
                catch
                {
                    MessageBox.Show("Failed to load image");
                }
            }
        }

        private void refreshBTN_Click(object sender, RoutedEventArgs e)
        {
            photosList.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(PhotoOpenClick));
            loadAlbumDetails();
        }

        private void addPicBTN_Click(object sender, RoutedEventArgs e)
        {
            GetUserNamePopup addAlbum = new GetUserNamePopup(true, this.opendAlbum, null, false, false, 0);
            addAlbum.Show();
        }
    }
}
