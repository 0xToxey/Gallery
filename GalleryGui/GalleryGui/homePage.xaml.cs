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

            List<string> albumsList = api.getAllAlbums();
            foreach (var album in albumsList)
            {
                ListViewItem name = new ListViewItem();
                name.Content = album;
                _albumsList.Items.Add(name);
            }

            _albumsList.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
        }

        private void loadPhotos()
        {
            // Get last photos.
            List<imgs>[] imgSource = new List<imgs>[4];
            List<string> imgLocations;
            int place = 0;

            try { imgLocations = api.getLastPhotos(); }
            catch (Exception err)
            {
                MessageBox.Show("Tap refresh button.\n" + err.Message);
                return;
            }
                
            // Display last photos.
            foreach (string source in imgLocations)
            {
                if (source != null)
                {
                    imgSource[place] = new List<imgs>();
                    imgSource[place].Add(new imgs() { ImageSource = source });
                    this._listViewArr[place].ItemsSource = imgSource[place];
                }

                place++;
            }
        }

        private void OnMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListBox).SelectedItem;
            if (item != null)
            {
                MessageBox.Show("**Open albhum here**");
            }
        }

    }

    public class imgs
    {
        public string ImageSource { get; set; }
    }

}
