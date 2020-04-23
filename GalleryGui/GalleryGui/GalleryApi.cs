using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace GalleryGui
{
    class GalleryApi
    {
        private string url = "http://127.0.0.1:3232/api/";

        #region Loggin/register things
        /*
         * Function check if user exist in database.
         */
        public bool userExist(string username)
        {
            // Create the get request msg
            string condition = url + "*/users/WHERE Name = '" + username + "'";

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JArray data = (JArray) json.SelectToken("data"); // Take the only the "data" 

            // If the data is empty, username not exist.
            if (data == null)
                return false;
            if (data.Count == 0)
                return false;

            return true;
        }

        /*
         * The function create new user.
         */
        public void createUser(string username, string password)
        {
            string urlCreate = this.url + "user/";
            string data = "name=" + username + "&password=" + password;

            string response;
            try
            {
                response = postRequest(urlCreate, data);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(response);

            if (json.ContainsKey("error"))
                throw new Exception("Error while creating the account.");
        }

        /*
         * Function check if password is correct.
         */
        public bool tryLoggin(string username, string password)
        {
            string urlCreate = this.url + "check/";
            string data = "username=" + username + "&password=" + password;

            string response;
            try
            {
                response = postRequest(urlCreate, data);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(response);
            JValue checkData = (JValue)json.SelectToken("data");

            if ((string)checkData.Value == "true")
                return true;
            return false;
        }
        
        /*
         * The function update password
         */
        public void changePass(string userid, string password)
        {
            string urlCreate = this.url + "user/" + userid;
            string data = "password=" + password;

            string response;
            try
            {
                response = patchRequest(urlCreate, data);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(response);

            if (json.ContainsKey("error"))
                throw new Exception("Error while changing password");

            else if (json.Count == 0)
                throw new Exception("Error while changing password");
        }
        #endregion

        #region Get things
        /*
         * The function reture the user id
         */
        public string getUserId(string username)
        {
            string userId;

            // Create the get request msg
            string condition = url + "*/users/WHERE Name = '" + username + "'";

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            var data = json.SelectToken("data"); // Take the only the "data"

            userId = data.First.SelectToken("ID").ToString();
            return userId;
        }

        /*
         * The function return list of all users 
         */
        public List<string> getAllUsers()
        {
            List<string> usersList = new List<string>();

            // Create the get request msg
            string condition = url + "*/users/null";

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JArray data = (JArray)json.SelectToken("data"); // Take the only the "data"

            foreach (var field in data)
            {
                string name = field.SelectToken("Name").ToString();
                usersList.Add(name);
            }

            return usersList;
        }

        /*
         * The function return all the albums
         */
        public List<Album> getAllAlbums()
        {
            List<Album> albumsList = new List<Album>();

            // Create the get request msg
            string condition = url + "*/Albums/null";

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JArray data = (JArray)json.SelectToken("data"); // Take the only the "data"

            foreach (var field in data)
            {
                Album Album = new Album();
                Album.name = field.SelectToken("Name").ToString();
                Album.ID = int.Parse(field.SelectToken("ID").ToString());
                Album.date = field.SelectToken("Creation_date").ToString();
                Album.ownerID = int.Parse(field.SelectToken("User_id").ToString());

                albumsList.Add(Album);
            }

            return albumsList;
        }

        /* 
         * The function return all the photos in album
         */
        public List<Img> getAlbumPhotos(Album album)
        {
            List<Img> photosList = new List<Img>();

            // Create the get request msg
            string condition = url + "*/Pictures/WHERE Album_id = " + album.ID;

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JArray data = (JArray)json.SelectToken("data"); // Take the only the "data"

            foreach (var field in data)
            {
                Img photo = new Img();
                photo.name = field.SelectToken("Name").ToString();
                photo.ID = int.Parse(field.SelectToken("ID").ToString());
                photo.date = field.SelectToken("Creation_date").ToString();
                photo.Path = field.SelectToken("Location").ToString();
                photo.albumID = int.Parse(field.SelectToken("Album_id").ToString());

                photosList.Add(photo);
            }

            return photosList;
        }

        /*
         * The function return an owner of photo
         */
        public int getPhotoOwner(Img photo)
        {
            int ownerID = 0;

            // Create the get request msg
            string condition = url + "Albums.User_id as owner/Albums/JOIN Pictures ON Pictures.Album_id = Albums.ID WHERE Pictures.ID = " + photo.ID;

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JArray data = (JArray)json.SelectToken("data"); // Take the only the "data"

            if (data != null)
            {
                ownerID = int.Parse(data.First.SelectToken("owner").ToString());
            }

            return ownerID;
        }

        /*
         * The function return the users that are tagged in picture.
         */
        public List<string> getTaggedUsers(Img photo)
        {
            List<string> usersList = new List<string>();

            // Create the get request msg
            string condition = url + "Users.Name/Users/JOIN Tags ON Users.ID = Tags.User_id WHERE Tags.Picture_id = " + photo.ID;

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JArray data = (JArray)json.SelectToken("data"); // Take the only the "data"

            foreach (var field in data)
            {
                string name = field.SelectToken("Name").ToString();
                usersList.Add(name);
            }

            return usersList;
        }

        /*
         * The function return the amount of tags in album
         */
        public int albumTagsAmount(Album album)
        {
            int amount = 0;

            // Create the get request msg
            string condition = url + "count(Tags.ID) as amount/Tags/JOIN Pictures ON Tags.Picture_id = Pictures.ID JOIN Albums ON Pictures.Album_id = Albums.ID WHERE Albums.id = " + album.ID;

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JArray data = (JArray)json.SelectToken("data"); // Take the only the "data"

            if (data != null)
            {
                amount = int.Parse(data.First.SelectToken("amount").ToString());
            }

            return amount;
        }

        /*
         * The function return the location of the last 4 photos.
         * (HomePage)
         */
        public List<Img> getLastPhotos()
        {
            List<Img> photosList = new List<Img>();

            // Create the get request msg
            string condition = url + "*/Pictures/LIMIT 4";

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JArray data = (JArray)json.SelectToken("data"); // Take the only the "data"

            foreach (var field in data)
            {
                Img photo = new Img();
                photo.name = field.SelectToken("Name").ToString();
                photo.ID = int.Parse(field.SelectToken("ID").ToString());
                photo.date = field.SelectToken("Creation_date").ToString();
                photo.Path = field.SelectToken("Location").ToString();
                photo.albumID = int.Parse(field.SelectToken("Album_id").ToString());

                photosList.Add(photo);
            }

            return photosList;
        }

        /*
         * The function return album data
         */
        public Album openAlbum(string albumName)
        {
            Album album = new Album();

            // Create the get request msg
            string condition = url + "*/Albums/WHERE Name = '" + albumName + "'";

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JArray data = (JArray)json.SelectToken("data"); // Take the only the "data"

            foreach (var field in data)
            {
                album.name = field.SelectToken("Name").ToString();
                album.ID = int.Parse(field.SelectToken("ID").ToString());
                album.date = field.SelectToken("Creation_date").ToString();
                album.ownerID = int.Parse(field.SelectToken("User_id").ToString());
            }

            return album;
        }

        /*
         * The function check if the name exist in the album
         */
        public bool CheckNameInAlbum(Album album, string nameToCheack)
        {
            List<Img> imgList = getAlbumPhotos(album);

            foreach(Img photo in imgList)
            {
                if (photo.name == nameToCheack)
                    return true;
            }

            return false;
        }

        /*
         * The function check if the album name is taken
         */
        public bool CheckAlbumName(string albumName)
        {
            List<Album> albums = getAllAlbums();

            foreach(Album album in albums)
            {
                if (album.name == albumName)
                    return true;
            }

            return false;
        }

        /*
         * The function return all the picture of user
         */
        public List<Img> getAllPhotosOf(string userid)
        {
            List<Img> userPhotos = new List<Img>();
            List<Album> userAlbums = getAllAlbumsOf(userid);

            foreach(Album album in userAlbums)
            {
                List<Img> newImgs = getAlbumPhotos(album);
                foreach (Img img in newImgs)
                {
                    userPhotos.Add(img);
                }
            }

            return userPhotos;
        }

        /*
         * The function return all the albums of user
         */
        public List<Album> getAllAlbumsOf(string userid)
        {
            List<Album> allAlbums = getAllAlbums();
            List<Album> userAlbums = new List<Album>();

            foreach(Album album in allAlbums)
            {
                if (album.ownerID.ToString() == userid)
                {
                    userAlbums.Add(album);
                }
            }

            return userAlbums;
        }

        /*
         * The function return the tags of user
         */
        public int getTagsCount(string userid)
        {
            int count = 0;

            // Create the get request msg
            string condition = url + "count(id) as count/Tags/WHERE user_id = " + userid;

            // Get the response from the server.
            string rsponse;
            try
            {
                rsponse = getRequest(condition);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JArray data = (JArray)json.SelectToken("data");

            if (data != null)
                count = int.Parse(data.First.SelectToken("count").ToString());

            return count;
        }

        #endregion

        #region Create things
        /*
         * The function create new album
         */
        public void createAlbum(Album newAlbum)
        {
            string urlCreate = this.url + "album/";
            string data = "name=" + newAlbum.name + "&creation_date=" + newAlbum.date + "&user_id=" + newAlbum.ownerID;

            string response;
            try
            {
                response = postRequest(urlCreate, data);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(response);

            if (json.ContainsKey("error"))
                throw new Exception("Error while creating the album.");
        }

        /*
         * The function create new photo
         */
        public void createImage(Img newPhoto)
        {
            string urlCreate = this.url + "picture/";
            string data = "name=" + newPhoto.name + "&creation_date=" + newPhoto.date + "&location=" + newPhoto.Path + "&album_id=" + newPhoto.albumID;

            string response;
            try
            {
                response = postRequest(urlCreate, data);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(response);

            if (json.ContainsKey("error"))
                throw new Exception("Error while creating the photo.");
        }

        /*
         * The function create new tag
         */
        public void addTag(string imgID, string userID)
        {
            string urlCreate = this.url + "tag/";
            string data = "picture_id=" + imgID + "&user_id=" + userID;

            string response;
            try
            {
                response = postRequest(urlCreate, data);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(response);

            if (json.ContainsKey("error"))
                throw new Exception("Error while creating tag.");
        }
        #endregion

        #region delete things.
        /*
         * The function delete a user.
         */
        public void deleteUser(string userId)
        {
            // Delete all users albums
            List<Album> userAlbums = getAllAlbumsOf(userId);
            foreach(Album album in userAlbums)
            {
                deleteAlbum(album);
            }

            string deleteURL = url + "user/" + userId;

            string response;
            try
            {
                response = deleteRequest(deleteURL);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(response);

            if (json.ContainsKey("error"))
                throw new Exception("Error deleting account.");
        }

        /*
         * The function delete an album
         */
        public void deleteAlbum(Album album)
        {
            List<Img> photosList = getAlbumPhotos(album);

            // Delete all photos in album
            foreach(Img photo in photosList)
            {
                deletePhoto(photo);
            }

            string deleteURL = url + "album/" + album.name;

            string response;
            try
            {
                response = deleteRequest(deleteURL);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(response);

            if (json.ContainsKey("error"))
                throw new Exception("Error deleting album.");
        }

        /*
         * The function delete a photo
         */
        public void deletePhoto(Img photo)
        {
            List<string> usersTaged = getTaggedUsers(photo);

            // Delete all tags in picture.
            foreach (string userName in usersTaged)
            {
                string userID = getUserId(userName);
                deleteTag(photo.ID.ToString(), userID);
            }

            string deleteURL = url + "picture/" + photo.albumID + "/" + photo.name;

            string response;
            try
            {
                response = deleteRequest(deleteURL);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(response);

            if (json.ContainsKey("error"))
                throw new Exception("Error deleting photo.");
        }

        /* 
         * The function delete a tag
         */
        public void deleteTag(string photoID, string userID)
        {
            string deleteURL = url + "tag/" + userID + "/" + photoID;

            string response;
            try
            {
                response = deleteRequest(deleteURL);
            }
            catch (Exception err)
            {
                throw err;
            }

            // Convert data to json.
            JObject json = JObject.Parse(response);

            if (json.ContainsKey("error"))
                throw new Exception("Error deleting tag.");
        }
        #endregion

        #region Post,Get,Delete,Patch Requests hendlers.

        /*
        * Function get the data from the PostRquest and conver it to string
        */
        private string postRequest(string url, string data)
        {
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
            List<string> dataSplit = data.Split('&').ToList();

            foreach (string pair in dataSplit)
            {
                string[] pairArr = pair.Split('=').ToArray<string>();
                postData.Add(new KeyValuePair<string, string>(pairArr[0], pairArr[1]));
            }

            string result;
            try
            {
                result = Task.Run(async () => await PostRequest(url, postData)).Result;
            }
            catch
            {
                throw new Exception("Something wrong with the server\n Please check that the server is on.");
            }

            return result;
        }

        /*
         * Function get the data from the GetRquest and conver it to string
         */
        private string getRequest(string data)
        {
            string result;
            try
            {
                result = Task.Run(async () => await GetRequest(data)).Result;
            }
            catch
            {
                throw new Exception("Something wrong with the server\n Please check that the server is on.");
            }

            return result;
        }

        /*
         * Function get the data from the PatchRquest and conver it to string
         */
        private string patchRequest(string url, string data)
        {
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
            List<string> dataSplit = data.Split('&').ToList();

            foreach (string pair in dataSplit)
            {
                string[] pairArr = pair.Split('=').ToArray<string>();
                postData.Add(new KeyValuePair<string, string>(pairArr[0], pairArr[1]));
            }

            string result;
            try
            {
                result = Task.Run(async () => await PatchRequest(url, postData)).Result;
            }
            catch
            {
                throw new Exception("Something wrong with the server\n Please check that the server is on.");
            }

            return result;
        }

        /*
         * Function get the data from the DeleteRquest and conver it to string
         */
        private string deleteRequest(string url)
        {
            string result;
            try
            {
                result = Task.Run(async () => await DeleteRequest(url)).Result;
            }
            catch
            {
                throw new Exception("Something wrong with the server\n Please check that the server is on.");
            }

            return result;
        }

        /*
         * Function handle with the Get request
         */
        private async static Task<string> GetRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent data = response.Content)
                    {
                        string content = await data.ReadAsStringAsync();
                        return content;
                    }
                }
            }
        }

        /*
         * Functionn handle with the Post request
         */
        private async static Task<string> PostRequest(string url, List<KeyValuePair<string, string>> postData)
        {

            IEnumerable<KeyValuePair<string, string>> queries = postData;
            HttpContent postDataContent = new FormUrlEncodedContent(queries);

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PostAsync(url, postDataContent))
                {
                    using (HttpContent data = response.Content)
                    {
                        string content = await data.ReadAsStringAsync();
                        return content;
                    }
                }
            }
        }

        /*
         * Functionn handle with the Patch request
         */
        private async static Task<string> PatchRequest(string url, List<KeyValuePair<string, string>> postData)
        {
            IEnumerable<KeyValuePair<string, string>> queries = postData;
            HttpContent postDataContent = new FormUrlEncodedContent(queries);
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await Patch.PatchAsync(client, url, postDataContent))
                {
                    using (HttpContent data = response.Content)
                    {
                        string content = await data.ReadAsStringAsync();
                        return content;
                    }
                }
            }
        }

        /*
        * Functionn handle with the Delete request
        */
        private async static Task<string> DeleteRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.DeleteAsync(url))
                {
                    using (HttpContent data = response.Content)
                    {
                        string content = await data.ReadAsStringAsync();
                        return content;
                    }
                }
            }
        }
        #endregion
    }

    static class Patch
    {
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent iContent)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = iContent
            };

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await client.SendAsync(request);
            }
            catch (TaskCanceledException e)
            {
                throw e;
            }

            return response;
        }

    }

    public class Img
    {
        public string Path { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public int albumID { get; set; }
        public int ID { get; set; }
    }

    public class Album
    { 
        public int ID { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public int ownerID { get; set; }
    }
}
