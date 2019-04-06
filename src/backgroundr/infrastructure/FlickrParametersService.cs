using System.IO;
using backgroundr.application;
using backgroundr.domain;

namespace backgroundr.infrastructure
{
    public class FlickrParametersService
    {
        private const string FILE_NAME = ".flickr";
        private readonly IFileService _fileService;

        public FlickrParametersService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public bool Exists()
        {
            return File.Exists(FILE_NAME);
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
