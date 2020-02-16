using System;
using System.IO;

namespace backgroundr.domain
{
    public class FlickrParametersService
    {
        private static readonly string FILE_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".config");

        private readonly IFileService _fileService;

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
            _fileService.Serialize(parameters, FILE_NAME);
        }

        public void SubscribeToChange()
        {
            _fileService.WhenFileChanged(FILE_NAME, () => { FlickrConfigurationFileChanged?.Invoke(); });
        }

        public void UnsubscribeToChange()
        {
            _fileService.StopWhenFileChanged(FILE_NAME);
        }
    }
}