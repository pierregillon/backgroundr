using System;
using System.Threading.Tasks;

namespace backgroundr.domain
{
    public interface IFileService
    {
        Task<string> Read(string filePath);
        Task Write(string filePath, string content);
        Task Append(string filePath, string content);
        Task Move(string source, string destination);
        bool Exists(string filePath);
        IFileWatching SubscribeToFileChange(string filePath, Action onModified);
        void UnsubscribeToFileChange(string filePath);
    }
}