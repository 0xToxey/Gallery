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
    /// Interaction logic for galleryPage.xaml
    /// </summary>
    public partial class galleryPage : Window
    {
        private readonly string _userName, _userId;
        
        public galleryPage(string user, string userid)
        {
            InitializeComponent();

            this._userId = userid;
            this._userName = user;
            UsernameDisplay.Text = user;

            // Start at home page.
            homebtn_Click(null, null);
        }

        #region ViewFunctions
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void closeBTN_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void updateCurrectMenu(string current)
        {
            if (current == "HOME")
            {
                SearchCurrect.Visibility = System.Windows.Visibility.Hidden;
                GalleyCurrect.Visibility = System.Windows.Visibility.Hidden;
                HomeCurrect.Visibility = System.Windows.Visibility.Visible;
            }

            else if (current == "SEARCH")
            {
                HomeCurrect.Visibility = System.Windows.Visibility.Hidden;
                GalleyCurrect.Visibility = System.Windows.Visibility.Hidden;
                SearchCurrect.Visibility = System.Windows.Visibility.Visible;
            }

            else if (current == "GALLERYS")
            {
                HomeCurrect.Visibility = System.Windows.Visibility.Hidden;
                SearchCurrect.Visibility = System.Windows.Visibility.Hidden;
                GalleyCurrect.Visibility = System.Windows.Visibility.Visible;
            }

            else if (current == "SETTINGS")
            {
                HomeCurrect.Visibility = System.Windows.Visibility.Hidden;
                SearchCurrect.Visibility = System.Windows.Visibility.Hidden;
                GalleyCurrect.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        #endregion

        private void homebtn_Click(object sender, RoutedEventArgs e)
        {
            updateCurrectMenu("HOME");
            pagesPlace.Children.Clear();

            homePage Child = new homePage(this._userName, this._userId);
            object content = Child.Content;
            Child.Content = null;
            Child.Close();
            this.pagesPlace.Children.Add(content as UIElement);
        }

        private void searchbtn_Click(object sender, RoutedEventArgs e)
        {
            updateCurrectMenu("SEARCH");
            pagesPlace.Children.Clear();

        }

        private void gallerysbtn_Click(object sender, RoutedEventArgs e)
        {
            updateCurrectMenu("GALLERYS");
            pagesPlace.Children.Clear();

        }

        private void settingsbtn_Click(object sender, RoutedEventArgs e)
        {
            updateCurrectMenu("SETTINGS");
            pagesPlace.Children.Clear();

            settingsPage Child = new settingsPage(this._userName, this._userId);
            object content = Child.Content;
            Child.Content = null;
            Child.Close();
            this.pagesPlace.Children.Add(content as UIElement);
        }
    }
}
