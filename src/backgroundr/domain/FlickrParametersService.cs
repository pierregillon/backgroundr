using System;
using System.IO;
using backgroundr.infrastructure;

namespace backgroundr.domain
{
    public class FlickrParametersService
    {
        private static readonly string FILE_NAME = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".backgroundr");
        
        private readonly IFileService _fileService;
        private FileWatcher _fileWatching;

        public event Action FlickrConfigurationFileChanged;

        public FlickrParametersService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public bool ConfigurationExists()
        {
            return _fileService.Exists(FILE_NAME);
        }

        public FlickrParameters Read()
        {
            return _fileService.Deserialize<FlickrParameters>(FILE_NAME);
        }

        public void Save(FlickrParameters parameters)
        {
            _fileWatching?.Pause();
            _fileService.Serialize(parameters, FILE_NAME);
            _fileWatching?.Start();
        }

        public void SubscribeToChange()
        {
            _fileWatching = _fileService.WhenFileChanged(FILE_NAME, () => {
                FlickrConfigurationFileChanged?.Invoke();
            });
        }

        public void UnsubscribeToChange()
        {
            _fileService.StopWhenFileChanged(FILE_NAME);
        }
    }
}