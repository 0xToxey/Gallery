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

        //foreach (var field in data)
        //{
        //    if (field.Key.ToString() == "Name")
        //    {
        //        string name = field.Value.ToString();
        //        if (name == username)
        //            return true;
        //    }
        //}

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
            JObject data = (JObject) json.SelectToken("data"); // Take the only the "data" 

            // If the data is empty, username not exist.
            if (data == null)
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

        #region Post&Get Requests hendlers.
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
            catch(Exception err)
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
            catch (Exception err)
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
        #endregion
    }
}
