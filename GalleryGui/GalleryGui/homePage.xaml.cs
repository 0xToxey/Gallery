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
        private ListBox _galleryList;
        private ListView _usersList;
        private GalleryApi api;

        public homePage(string username, string userid)
        {
            InitializeComponent();
            this._userName = username;
            this._userId = userid;
            this._galleryList = galleryList;
            this._usersList = usersList;
            this.api = new GalleryApi();


            RefreshLists();
        }

        private void RefreshLists()
        {
            loadUsers();
        }

        private void refreshBTN_Click(object sender, RoutedEventArgs e)
        {
            RefreshLists();
        }

        private void loadUsers()
        {
            this._usersList.Items.Clear();
            this._usersList.DisplayMemberPath = "Text";

            List<string> usersList = api.getAllUsers();
            foreach (var user in usersList)
            {
                if (user != this._userName)
                {
                    ListViewItem name = new ListViewItem();
                    name.Content = user;
                    this._usersList.Items.Add(name);
                }
            }
        }

    }
}
