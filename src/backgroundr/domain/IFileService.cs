using System;

namespace backgroundr.domain
{
    public interface IFileService
    {
        string Read(string filePath);
        void Write(string filePath, string content);
        void Append(string filePath, string content);
        bool Exists(string filePath);
        IFileWatching SubscribeToFileChange(string filePath, Action onModified);
        void UnsubscribeToFileChange(string filePath);
    }
}