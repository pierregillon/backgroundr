using System;
using backgroundr.infrastructure;

namespace backgroundr.domain
{
    public interface IFileService
    {
        T Deserialize<T>(string filePath);
        void Serialize<T>(T obj, string filePath);
        bool Exists(string filePath);
        void Append(string fileName, string content);
        FileWatcher WhenFileChanged(string fileName, Action action);
        void StopWhenFileChanged(string fileName);
    }
}