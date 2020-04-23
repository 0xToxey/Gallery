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
using System.Globalization;

namespace GalleryGui
{
    /// <summary>
    /// Interaction logic for GetUserNamePopup.xaml
    /// </summary>
    public partial class GetUserNamePopup : Window
    {
        private GalleryApi api;

        private bool _NewPic;
        private Album _album;

        private Img _photo;
        private bool _tag;

        private bool _newAlbum;
        private int _ownerID;

        public GetUserNamePopup(bool NewPic, Album album, Img photo, bool tag, bool newAlbum, int ownerID)
        {
            InitializeComponent();

            this.api = new GalleryApi();
            this._NewPic = NewPic;
            this._album = album;
            this._photo = photo;
            this._tag = tag;
            this._newAlbum = newAlbum;
            this._ownerID = ownerID;

            // Id its Un/Tag user
            if (!newAlbum)
                if (!this._NewPic)
                {
                    Un_Tag_User.Visibility = System.Windows.Visibility.Visible;
                }
                else // If its new picture.
                {
                    AddPhoto.Visibility = System.Windows.Visibility.Visible;
                }
            else
            {
                NewAlbum.Visibility = System.Windows.Visibility.Visible;
            }
        }

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

        private void PicName_GotFocus(object sender, RoutedEventArgs e)
        {
            PicName.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void Location_GotFocus(object sender, RoutedEventArgs e)
        {
            Location.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void Username_GotFocus(object sender, RoutedEventArgs e)
        {
            Username.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void AlbumName_GotFocus(object sender, RoutedEventArgs e)
        {
            AlbumName.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void acceptBTN_Click(object sender, RoutedEventArgs e)
        {
            if (this._newAlbum)// If its new albums option.
            {
                if (AlbumName.Text.Length == 0)
                {
                    AlbumName.BorderBrush = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("Album Name Empty");
                    return;
                }
                else if (api.CheckAlbumName(AlbumName.Text))
                {
                    AlbumName.BorderBrush = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("Album Name Taken");
                    return;
                }
                else
                {
                    Album newAlbum = new Album();
                    newAlbum.name = AlbumName.Text;
                    newAlbum.ownerID = this._ownerID;

                    var dataTime = new CultureInfo("en-GB");
                    DateTime localDate = DateTime.Now;
                    newAlbum.date = localDate.ToString(dataTime);
                    try
                    {
                        api.createAlbum(newAlbum);
                        MessageBox.Show("New Album was added.");
                        this.Close();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }
                }
            }
            else if (!this._NewPic) // If its Un/Tag user
            {
                if (Username.Text.Length == 0)
                {
                    Username.BorderBrush = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("UserName Empty");
                }
                else if (!api.userExist(Username.Text))
                {
                    Username.BorderBrush = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("UserName dosent exist.");
                }
                else
                {
                    if (!this._tag)
                    {
                        List<string> users = api.getTaggedUsers(this._photo);
                        if (users.IndexOf(Username.Text) == -1)
                        {
                            Username.BorderBrush = System.Windows.Media.Brushes.Red;
                            MessageBox.Show("UserName isnt tagged.");
                            return;
                        }
                    }
                    else
                    {
                        List<string> users = api.getTaggedUsers(this._photo);
                        if (users.IndexOf(Username.Text) != -1)
                        {
                            Username.BorderBrush = System.Windows.Media.Brushes.Red;
                            MessageBox.Show("UserName is tagged.");
                            return;
                        }
                    }
                    try
                    {
                        if (this._tag)
                        {
                            api.addTag(_photo.ID.ToString(), api.getUserId(Username.Text));
                            MessageBox.Show("Tag was created.");
                            this.Close();
                        }
                        else
                        {
                            api.deleteTag(_photo.ID.ToString(), api.getUserId(Username.Text));
                            MessageBox.Show("User was untaged.");
                            this.Close();
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                        this.Close();
                    }
                }
            }
            else // If its new picture.
            {
                if (PicName.Text.Length == 0)
                {
                    PicName.BorderBrush = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("Photo Name empty.");
                    return;
                }
                else if (api.CheckNameInAlbum(this._album, PicName.Text))
                {
                    PicName.BorderBrush = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("Name cannot be use.");
                    return;
                }
                else if (Location.Text.Length == 0)
                {
                    Location.BorderBrush = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("Location empty.");
                    return;
                }
                else if (!System.IO.File.Exists(Location.Text))
                {
                    Location.BorderBrush = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("File doesnt exist.");
                    return;
                }
                else
                {
                    Img newPhoto = new Img();
                    newPhoto.albumID = this._album.ID;
                    newPhoto.Path = Location.Text;
                    newPhoto.name = PicName.Text;
                    var dataTime = new CultureInfo("en-GB");
                    DateTime localDate = DateTime.Now;
                    newPhoto.date = localDate.ToString(dataTime);
                    try
                    {
                        api.createImage(newPhoto);
                        MessageBox.Show("New photo was added.");
                        this.Close();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }
                }
            }
        }
    }
}
