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
    /// Interaction logic for homePage.xaml
    /// </summary>
    public partial class homePage : Window
    {
        private string _userName, _userId;
        private ListBox _albumsList;
        private ListView _usersList;
        private GalleryApi api;
        private ListView[] _listViewArr;
        private List<img> _imgsList;

        public homePage(string username, string userid)
        {
            InitializeComponent();
            this._userName = username;
            this._userId = userid;
            this._albumsList = albumsList;
            this._usersList = usersList;
            this.api = new GalleryApi();

            _listViewArr = new ListView[4];
            _listViewArr[0] = photo1;
            _listViewArr[1] = photo2;
            _listViewArr[2] = photo3;
            _listViewArr[3] = photo4;


            RefreshLists();
        }

        private void RefreshLists()
        {
            loadUsers();
            loadPhotos();
            loadAlbums();
        }

        private void refreshBTN_Click(object sender, RoutedEventArgs e)
        {
            RefreshLists();
        }

        private void loadUsers()
        {
            this._usersList.Items.Clear();
            _usersList.DisplayMemberPath = "Text";

            List<string> usersList = api.getAllUsers();
            foreach (var user in usersList)
            {
                if (user != this._userName)
                {
                    ListViewItem name = new ListViewItem();
                    name.Content = user;
                    _usersList.Items.Add(name);
                }
            }
        }

        private void loadAlbums()
        {
            this._albumsList.Items.Clear();
            _albumsList.DisplayMemberPath = "Text";

            List<album> albumsList = api.getAllAlbums();
            foreach (album albumVar in albumsList)
            {
                ListViewItem name = new ListViewItem();
                name.Content = albumVar.name;
                _albumsList.Items.Add(name);
            }

            _albumsList.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(AlbumOpenClick), true);
        }

        private void loadPhotos()
        {
            // Get last photos.
            this._imgsList = new List<img>();
            int place = 0;

            try { _imgsList = api.getLastPhotos(); }
            catch (Exception err)
            {
                MessageBox.Show("Tap refresh button.\n" + err.Message);
                return;
            }
                
            // Display last photos.
            foreach (img photo in _imgsList)
            {
                List<img> tempImg = new List<img>();
                tempImg.Add(photo);

                this._listViewArr[place].ItemsSource = tempImg;

                _listViewArr[place].AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(PhotoOpenClick), true);
                place++;
            }
        }

        private void AlbumOpenClick(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListBox).SelectedItem;
            if (item != null)
            {
                MessageBox.Show("**Open albu, here**");
            }
        }

        private void PhotoOpenClick(object sender, RoutedEventArgs e)
        {
            img item = ((sender as ListView).SelectedItem as img);
            if (item != null)
            {
                try
                {
                    ShowPhoto photoOpen = new ShowPhoto(item);
                    photoOpen.Show();
                }
                catch
                {
                    MessageBox.Show("Failed to load image");
                }
            }
        }
    }

    class ImageDisplay
    {
        public string ImageSource { get; set; }
    }
}
