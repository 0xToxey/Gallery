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
        private readonly string _username;

        public galleryPage(string user)
        {
            InitializeComponent();
            this._username = user;
            updateCurrectMenu("HOME");
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
        }

        private void closeBTN_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void homebtn_Click(object sender, RoutedEventArgs e)
        {
            updateCurrectMenu("HOME");
        }

        private void searchbtn_Click(object sender, RoutedEventArgs e)
        {
            updateCurrectMenu("SEARCH");
        }

        private void gallerysbtn_Click(object sender, RoutedEventArgs e)
        {
            updateCurrectMenu("GALLERYS");
        }
    }
}
