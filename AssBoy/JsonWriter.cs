using System.Collections.Generic;
using System.Collections.Specialized;

namespace Kwartet.Desktop
{
    public class JsonBuilder
    {
        private Dictionary<string, string> @params = new Dictionary<string, string>();
        
        protected JsonBuilder() { }

        public static JsonBuilder Create()
        {
            return new JsonBuilder();
        }

        public JsonBuilder WithParam(string key, string value)
        {
            @params.Add(key, value);
            return this;
        }

        public JsonBuilder WithParam(string key, IEnumerable<object> array)
        {
            string arrayJson = "[";

            bool firstTime = true;
            foreach (var a in array)
            {
                if (!firstTime)
                    arrayJson += ",";

                arrayJson += $"'{a.ToString()}'";
                
                firstTime = false;
            }
            
            arrayJson += "]";

            @params.Add(key, arrayJson);
            return this;
        }
        
        /// <summary>
        /// Converts the JSON to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string json = "{";

            bool isFirst = true;
            foreach (var part in @params)
            {
                if (!isFirst)
                {
                    json += ",";
                }

                json += $"'{part.Key}':'{part.Value}'";
                
                isFirst = false;
            }
            
            json += "}";

            return json;
        }
    }
}