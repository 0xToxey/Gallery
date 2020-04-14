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

        /*
         * Function check if user exist in database.
         */
        public bool userExist(string username)
        {
            // Create the get request msg
            string condition = url + "*/users/WHERE Name = '" + username + "'";
            
            // Get the response from the server.
            string rsponse = getRequest(condition);

            // Convert data to json.
            JObject json = JObject.Parse(rsponse);
            JObject data = (JObject) json.SelectToken("data"); // Take the only the "data" 

            // If the data is empty, username not exist.
            if (data == null)
                return false;

            return true;
            
            //foreach (var field in data)
            //{
            //    if (field.Key.ToString() == "Name")
            //    {
            //        string name = field.Value.ToString();
            //        if (name == username)
            //            return true;
            //    }
            //}
        }




        public string getRequest(string data)
        {
            string result = Task.Run(async () => await GetRequest(data)).Result;
            return result;
        }

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
    }
}
