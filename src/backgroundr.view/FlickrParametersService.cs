using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace backgroundr.view
{
    public class FlickrParametersService
    {
        private static readonly string FILE_NAME = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".backgroundr");

        public FlickrParameters Read()
        {
            return JsonConvert.DeserializeObject<FlickrParameters>(File.ReadAllText(FILE_NAME));
        }

        public void Save(FlickrParameters parameters)
        {
            var newJson = JObject.FromObject(parameters);
            var oldJson = JObject.Parse(File.ReadAllText(FILE_NAME));

            oldJson.Merge(newJson, new JsonMergeSettings {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            File.WriteAllText(FILE_NAME, oldJson.ToString());
        }

        public void ResetBackgroundImageLastRefreshDate()
        {
            var json = JObject.Parse(File.ReadAllText(FILE_NAME));
            json.Remove(nameof(FlickrParameters.BackgroundImageLastRefreshDate));
            File.WriteAllText(FILE_NAME, json.ToString());
        }
    }
}