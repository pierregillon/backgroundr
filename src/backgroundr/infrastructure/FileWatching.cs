using System;
using System.IO;
using backgroundr.domain;

namespace backgroundr.infrastructure
{
    public class FileWatching : IFileWatching
    {
        private static readonly TimeSpan ONE_SECOND = TimeSpan.FromSeconds(1);

        private DateTime _lastWriteTimeSaved = DateTime.MinValue;
        private readonly FileSystemWatcher _watcher;

        public FileWatching(string fileName, Action onModified)
        {
            _watcher = BuildWatcher(fileName, onModified);
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void Pause()
        {
            _watcher.EnableRaisingEvents = false;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
        }

        private FileSystemWatcher BuildWatcher(string filePath, Action onModified)
        {
            var watcher = new FileSystemWatcher {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(filePath),
                Path = Directory.GetParent(filePath).FullName
            };
            watcher.Changed += (sender, args) => {
                if (HasBeenModified(filePath)) {
                    onModified();
                }
            };
            return watcher;
        }

        private bool HasBeenModified(string filePath)
        {
            var lastWriteTime = File.GetLastWriteTime(filePath);
            var elapsedTimeFromLastWrite = lastWriteTime.Subtract(_lastWriteTimeSaved);
            if (elapsedTimeFromLastWrite <= ONE_SECOND) {
                return false;
            }
            _lastWriteTimeSaved = lastWriteTime;
            return true;
        }
    }
}