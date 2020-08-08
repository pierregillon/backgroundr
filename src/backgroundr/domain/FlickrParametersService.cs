using System;
using System.IO;
using backgroundr.domain.events;
using ddd_cqrs;
using Newtonsoft.Json;

namespace backgroundr.domain
{
    public class FlickrParametersService
    {
        private static readonly string FILE_NAME = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".backgroundr");

        private readonly IFileService _fileService;
        private readonly IEventEmitter _eventEmitter;
        private IFileWatching _fileWatching;

        public FlickrParametersService(IFileService fileService, IEventEmitter eventEmitter)
        {
            _fileService = fileService;
            _eventEmitter = eventEmitter;
        }

        public bool ConfigurationExists()
        {
            return _fileService.Exists(FILE_NAME);
        }

        public FlickrParameters Read()
        {
            return JsonConvert.DeserializeObject<FlickrParameters>(_fileService.Read(FILE_NAME));
        }

        public void Save(FlickrParameters parameters)
        {
            _fileWatching?.Pause();
            _fileService.Write(FILE_NAME, JsonConvert.SerializeObject(parameters, Formatting.Indented));
            _fileWatching?.Start();
        }

        public void SubscribeToChange()
        {
            _fileWatching = _fileService.SubscribeToFileChange(FILE_NAME, () => _eventEmitter.Emit(new FileConfigurationModified()));
        }

        public void UnsubscribeToChange()
        {
            _fileService.UnsubscribeToFileChange(FILE_NAME);
            _fileWatching = null;
        }
    }
}