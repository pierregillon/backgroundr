using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using backgroundr.domain;
using Newtonsoft.Json;

namespace backgroundr.infrastructure
{
    public class FileService : IFileService
    {
        private readonly IDictionary<string, FileWatcher> _dictionary = new Dictionary<string, FileWatcher>();

        public T Deserialize<T>(string filePath)
        {
            return JsonConvert.DeserializeObject<T>(SafeReadAllText(filePath));
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

        public void WhenFileChanged(string fileName, Action action)
        {
            if (_dictionary.ContainsKey(fileName)) {
                throw new InvalidOperationException("File already watched.");
            }

            _dictionary.Add(fileName, new FileWatcher(fileName, action));
            _dictionary[fileName].Start();
        }

        public void StopWhenFileChanged(string fileName)
        {
            if (!_dictionary.ContainsKey(fileName)) {
                throw new InvalidOperationException("File not watched.");
            }

            _dictionary[fileName].Stop();
            _dictionary.Remove(fileName);
        }

        private static string SafeReadAllText(string filePath, int attempt = 10)
        {
            try {
                return File.ReadAllText(filePath);
            }
            catch (IOException ex) {
                if (attempt == 0) {
                    throw ex;
                }

                Thread.Sleep(10);
                return SafeReadAllText(filePath, attempt - 1);
            }
        }
    }
}