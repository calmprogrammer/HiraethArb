using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HiraethArb.BusinessLogic.Classes
{
    public abstract class API
    {
        public API(string url)
        {
            this.url = url;

        }

        protected virtual JsonNode? jsonObject { get; set; }
        protected virtual string url { get; set; }


        public virtual void GetAPI()
        {
            using var httpClient = new HttpClient();
            try
            {
                var response = httpClient.GetStringAsync(new Uri(url)).Result;
                JsonNode? jsonObject = JsonNode.Parse(response);
                this.jsonObject = jsonObject!;
                Deserialize();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error :  " + e.Message);
            }

        }

        protected abstract void Deserialize();

    }
}
