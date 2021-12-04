using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this T data, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(data, formatting);
        }

        public static T ToDeserialize<T>(this string jsonString)
        {
            return !jsonString.HasValue() ? default : JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static T GetValue<T>(this JObject jObject, string key, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (jObject == null)
                return default(T);

            foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
            {
                if (keyValuePair.Key.Equals(key, stringComparison))
                {
                    return (T)keyValuePair.Value.ToObject(typeof(T));
                }
            }
            return default(T);
        }

        public static string MergeJson(this string oldJson, string newJson)
        {
            var dataBaseAttributes = JObject.Parse(oldJson);
            var uiAttributes = JObject.Parse(newJson);

            dataBaseAttributes.Merge(uiAttributes, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });
            return dataBaseAttributes.ToString();
        }
    }
}
