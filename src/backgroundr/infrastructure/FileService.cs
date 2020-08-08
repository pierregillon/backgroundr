using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using backgroundr.domain;

namespace backgroundr.infrastructure
{
    public class FileService : IFileService
    {
        private readonly IDictionary<string, FileWatching> _dictionary = new Dictionary<string, FileWatching>();

        public string Read(string filePath)
        {
            return SafeReadAllText(filePath);
        }

        public void Write(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }

        public void Append(string filePath, string content)
        {
            File.AppendAllText(filePath, content);
        }

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public IFileWatching SubscribeToFileChange(string fileName, Action onModified)
        {
            if (_dictionary.ContainsKey(fileName)) {
                throw new InvalidOperationException("File already watched.");
            }

            _dictionary.Add(fileName, new FileWatching(fileName, onModified));
            _dictionary[fileName].Start();
            return _dictionary[fileName];
        }

        public void UnsubscribeToFileChange(string filePath)
        {
            if (!_dictionary.ContainsKey(filePath)) {
                throw new InvalidOperationException("File not watched.");
            }

            _dictionary[filePath].Stop();
            _dictionary.Remove(filePath);
        }

        private static string SafeReadAllText(string filePath, int attempt = 10)
        {
            try {
                return File.ReadAllText(filePath);
            }
            catch (IOException) {
                if (attempt == 0) {
                    throw;
                }

                Thread.Sleep(10);
                return SafeReadAllText(filePath, attempt - 1);
            }
        }
    }
}