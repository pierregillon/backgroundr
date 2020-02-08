using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace backgroundr.view
{
    public class FlickrParametersService
    {
        public FlickrParameters Read()
        {
            return JsonConvert.DeserializeObject<FlickrParameters>(File.ReadAllText(FindDaemonConfigurationFile()));
        }

        public void Save(FlickrParameters parameters)
        {
            var newJson = JObject.FromObject(parameters);
            var oldJson = JObject.Parse(File.ReadAllText(FindDaemonConfigurationFile()));

            oldJson.Merge(newJson, new JsonMergeSettings {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            File.WriteAllText(FindDaemonConfigurationFile(), oldJson.ToString());
        }

        private static string FindDaemonConfigurationFile()
        {
            var process = Process.GetProcessesByName("backgroundr.daemon").FirstOrDefault();
            if (process == null) {
                throw new InvalidOperationException("Daemon process was not found");
            }

            return Path.Combine(process.StartInfo.WorkingDirectory, ".config");
        }
    }
}