using System;
using System.IO;

namespace backgroundr.infrastructure
{
    public class FileWatcher
    {
        private DateTime _lastFileRead = DateTime.MinValue;
        private readonly FileSystemWatcher _watcher;

        public FileWatcher(string fileName, Action action)
        {
            _watcher = new FileSystemWatcher {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(fileName),
                Path = Directory.GetParent(fileName).FullName
            };
            _watcher.Changed += (sender, args) => {
                var lastWriteTime = File.GetLastWriteTime(args.FullPath);
                if (lastWriteTime.Subtract(_lastFileRead).TotalSeconds > 1) {
                    action();
                    _lastFileRead = lastWriteTime;
                }
            };
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
    }
}