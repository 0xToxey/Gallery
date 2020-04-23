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
    /// Interaction logic for settingsPage.xaml
    /// </summary>
    public partial class settingsPage : Window
    {
        private string _username, _userId;
        private GalleryApi api;

        public settingsPage(string username, string userid)
        {
            InitializeComponent();
            this.api = new GalleryApi();
            this._username = username;
            this._userId = userid;

            displaySettings();
        }

        #region ViewFunctions
        private void closeBTN_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void displaySettings()
        {
            Username.Text += this._username;
            UserID.Text += this._userId;
        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            Password.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void CheckPassword_Click(object sender, RoutedEventArgs e)
        {
            CheckPassword.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }
        #endregion

        private void change_btn_Click(object sender, RoutedEventArgs e)
        {   
            GalleryApi api = new GalleryApi();

            // Password & pass check dont match.
            if (CheckPassword.Password.ToString() != Password.Text)
            {
                Password.BorderBrush = System.Windows.Media.Brushes.Red;
                CheckPassword.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Passwords don't match.");
            }

            // Check if pass is too short.
            else if (Password.Text.Length < 6)
            {
                // Check that password box arent empty.
                if (Password.Text.Length == 0)
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

            // If all is correct
            else
            {
                Password.BorderBrush = System.Windows.Media.Brushes.Transparent;
                CheckPassword.BorderBrush = System.Windows.Media.Brushes.Transparent;

                try
                {
                    api.changePass(this._userId, Password.Text);
                    MessageBox.Show("Password was changed.");
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void logout_btn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                api.deleteUser(this._userId);
                MessageBox.Show("Account deleted!");
                Application.Current.Shutdown();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
