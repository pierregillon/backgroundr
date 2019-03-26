using System.IO;
using backgroundr.domain;
using Newtonsoft.Json;

namespace backgroundr.infrastructure
{
    public class FileService : IFileService
    {
        public T Deserialize<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Serialize<T>(T obj, string filePath)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }
        public void Append(string fileName, string content)
        {
            File.AppendAllText(fileName, content);
        }
    }
}