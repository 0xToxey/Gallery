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
    /// Interaction logic for profilePage.xaml
    /// </summary>
    public partial class profilePage : Window
    {
        private GalleryApi api;

        private string _userName, _userID;


        public profilePage(string userName, string userID)
        {
            InitializeComponent();
            api = new GalleryApi();

            this._userName = userName;
            this._userID = userID;

            RefreshLists();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void refreshBTN_Click(object sender, RoutedEventArgs e)
        {
            RefreshLists();
        }

        private void RefreshLists()
        {
            albumsList.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(AlbumOpenClick));
            photosList.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(PhotoOpenClick));

            AlbumAmount.Text = "Albums amount: " + api.getAllAlbumsOf(_userID).Count();
            PhotosAmount.Text = "Photos amount: " + api.getAllPhotosOf(_userID).Count();
            TagAmount.Text = "Tags amount: " + api.getTagsCount(_userID);

            loadAlbums();
            loadPhotos();
        }

        private void loadPhotos()
        {
            List<Img> imgList = api.getAllPhotosOf(_userID);
            photosList.Items.Clear();

            foreach (Img img in imgList)
            {
                List<Img> tempImg = new List<Img>();
                tempImg.Add(img);

                photosList.Items.Add(tempImg);
            }
            photosList.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(PhotoOpenClick), true);
        }

        private void loadAlbums()
        {
            this.albumsList.Items.Clear();
            albumsList.DisplayMemberPath = "Text";

            List<Album> userAlbumsList = api.getAllAlbumsOf(_userID);
            foreach (Album album in userAlbumsList)
            {
                ListViewItem name = new ListViewItem();
                name.Content = album.name;

                albumsList.Items.Add(name);
            }

            albumsList.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(AlbumOpenClick), true);
        }

        private void PhotoOpenClick(object sender, RoutedEventArgs e)
        {
            Img item = ((sender as ListBox).SelectedItem as List<Img>)[0];
            if (item != null)
            {
                try
                {
                    ShowPhoto photoOpen = new ShowPhoto(item, _userID);
                    photoOpen.Show();
                }
                catch
                {
                    MessageBox.Show("Failed to load image");
                }
            }
        }

        private void AlbumOpenClick(object sender, RoutedEventArgs e)
        {
            ListViewItem item = (sender as ListBox).SelectedItem as ListViewItem;
            if (item != null)
            {
                try
                {
                    Album album = api.openAlbum(item.Content.ToString());
                    ShowAlbum photoOpen = new ShowAlbum(album, _userID);
                    photoOpen.Show();
                }
                catch
                {
                    MessageBox.Show("Failed to load album");
                }
            }
        }

        private void addAlbumBTN_Click(object sender, RoutedEventArgs e)
        {
            GetUserNamePopup addAlbum = new GetUserNamePopup(false, null, null, false, true, int.Parse(this._userID));
            addAlbum.Show();
        }
    }
}
