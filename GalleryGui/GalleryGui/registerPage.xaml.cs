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
    /// Interaction logic for registerPage.xaml
    /// </summary>
    public partial class registerPage : Window
    {
        public registerPage()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Username_Click(object sender, MouseButtonEventArgs e)
        {
            Username.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void Password_Click(object sender, MouseButtonEventArgs e)
        {
            Password.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void CheckPassword_Click(object sender, MouseButtonEventArgs e)
        {
            CheckPassword.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void closeBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CompleteRegisterBTN_Click(object sender, RoutedEventArgs e)
        {
            GalleryApi api = new GalleryApi();

            // If user already exist
            if (api.userExist(Username.Text))
            {
                MessageBox.Show("Username is taken.");
                Username.BorderBrush = System.Windows.Media.Brushes.Red;
            }

            // Check if pass is too short.
            else if (Password.Password.Length < 6)
            {
                MessageBox.Show("Passwords too short.");
                Password.BorderBrush = System.Windows.Media.Brushes.Red;
                CheckPassword.BorderBrush = System.Windows.Media.Brushes.Red;
            }
         
            // Password & pass check dont match.
            else if (CheckPassword.Password.ToString() != Password.Password.ToString())
            {
                MessageBox.Show("Passwords don't match.");
                Password.BorderBrush = System.Windows.Media.Brushes.Red;
                CheckPassword.BorderBrush = System.Windows.Media.Brushes.Red;
            }

            // Check if username is taken.
            //else if (userExist)
            //{
            //    var responseString = await client.GetStringAsync("http://www.example.com/recepticle.aspx");
            //}

            else
            {
                Password.BorderBrush = System.Windows.Media.Brushes.Transparent;
                CheckPassword.BorderBrush = System.Windows.Media.Brushes.Transparent;

                MessageBox.Show("New account created.");

                // Enter with new user.
                MainWindow login = new MainWindow();
                login.Show();

                this.Close();
            }
        }

    }
}
