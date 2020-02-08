using System;
using System.IO;
using System.Threading;
using backgroundr.domain;
using Newtonsoft.Json;

namespace backgroundr.infrastructure
{
    public class FileService : IFileService
    {
        private readonly DateTime _lastRead = DateTime.MinValue;

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
            var watcher = new FileSystemWatcher {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(fileName),
                Path = Directory.GetParent(fileName).FullName
            };
            watcher.Changed += (sender, args) => {
                var lastWriteTime = File.GetLastWriteTime(args.FullPath);
                if (lastWriteTime != _lastRead) {
                    action();
                }
            };
            watcher.EnableRaisingEvents = true;
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