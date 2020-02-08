using System;

namespace backgroundr.domain
{
    public interface IFileService
    {
        T Deserialize<T>(string filePath);
        void Serialize<T>(T obj, string filePath);
        bool Exists(string filePath);
        void Append(string fileName, string content);
        void WhenFileChanged(string fileName, Action action);
    }
}