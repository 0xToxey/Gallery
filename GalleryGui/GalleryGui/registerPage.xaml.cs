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
            this.Close();
        }
        #endregion

        #region ClearBorder
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
        #endregion

        private void CompleteRegisterBTN_Click(object sender, RoutedEventArgs e)
        {
            GalleryApi api = new GalleryApi();

            // Check that username box arent empty.
            if (Username.Text.Length == 0)
                Username.BorderBrush = System.Windows.Media.Brushes.Red;

            // If user already exist
            else if (api.userExist(Username.Text))
            {
                Username.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Username is taken.");
            }

            // Check if pass is too short.
            else if (Password.Password.Length < 6)
            {
                // Check that password box arent empty.
                if (Password.Password.Length == 0)
                    Password.BorderBrush = System.Windows.Media.Brushes.Red;
                
                else if (CheckPassword.Password.Length == 0)
                    CheckPassword.BorderBrush = System.Windows.Media.Brushes.Red;
                
                else
                {
                    Password.BorderBrush = System.Windows.Media.Brushes.Red;
                    CheckPassword.BorderBrush = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("Passwords too short.");
                }
            }
         
            // Password & pass check dont match.
            else if (CheckPassword.Password.ToString() != Password.Password.ToString())
            {
                Password.BorderBrush = System.Windows.Media.Brushes.Red;
                CheckPassword.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Passwords don't match.");
            }

            // If all is correct
            else
            {
                Password.BorderBrush = System.Windows.Media.Brushes.Transparent;
                CheckPassword.BorderBrush = System.Windows.Media.Brushes.Transparent;
                Username.BorderBrush = System.Windows.Media.Brushes.Transparent;

                try
                {
                    api.createUser(Username.Text, Password.Password.ToString());

                    MessageBox.Show("New account created.");
                    // Enter with new user.
                    MainWindow login = new MainWindow();
                    login.Show();
                    this.Close();
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message);
                    MainWindow loginPage = new MainWindow();
                    loginPage.Show();

                    this.Close();
                }
            }
        }
    }
}
