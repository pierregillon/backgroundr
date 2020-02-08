using System;
using System.IO;

namespace backgroundr.domain
{
    public class FlickrParametersService
    {
        private static readonly string FILE_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".config");

        private readonly IFileService _fileService;

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
    }
}
