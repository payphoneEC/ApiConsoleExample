using Newtonsoft.Json;

namespace ApiConnectionExample
{
    public class JsonConversion
    {
        /// <summary>
        /// Convierte un objeto en json
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string ObjectToJson(object param)
        {
            return JsonConvert.SerializeObject(param);
        }

        /// <summary>
        /// </summary>
        /// <param name="json"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T JsonToObject<T>(string json)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
