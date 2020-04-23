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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GalleryGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region PageView
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
        #endregion

        #region ClearBorder
        private void Username_Click(object sender, RoutedEventArgs e)
        {
            Username.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            Password.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }
        #endregion

        private void RegisterBTN_Click(object sender, RoutedEventArgs e)
        {
            registerPage register = new registerPage();
            register.Show();
            this.Close();
        }


        private void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            GalleryApi api = new GalleryApi();

            try
            {
                bool success = api.tryLoggin(Username.Text, Password.Password.ToString());

                if (success)
                {
                    string userId = api.getUserId(Username.Text);

                    galleryPage gallery = new galleryPage(Username.Text, userId);
                    gallery.Show();
                    this.Close();
                }
                else
                {
                    Username.BorderBrush = System.Windows.Media.Brushes.Red;
                    Password.BorderBrush = System.Windows.Media.Brushes.Red;

                    MessageBox.Show("Username and Password dont match.");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
