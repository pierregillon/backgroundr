using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using backgroundr.domain;
using backgroundr.domain.events;
using backgroundr.infrastructure;
using ddd_cqrs;

namespace backgroundr.application.changeDesktopBackgroundImageRandomly
{
    public class ChangeDesktopBackgroundImageRandomlyCommandHandler : ICommandHandler<ChangeDesktopBackgroundImageRandomlyCommand>
    {
        private static readonly string IMAGES_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "backgroundr");
        
        private readonly IDesktopBackgroundImageUpdater _desktopBackgroundImageUpdater;
        private readonly IPhotoProvider _photoProvider;
        private readonly IFileDownloader _fileDownloader;
        private readonly IFileService _fileService;
        private readonly IRandom _random;
        private readonly IEventEmitter _eventEmitter;
        private readonly FlickrParameters _flickrParameters;
        private readonly IClock _clock;
        private readonly FlickrParametersService _flickrParametersService;

        // ----- Constructor

        public ChangeDesktopBackgroundImageRandomlyCommandHandler(
            IDesktopBackgroundImageUpdater desktopBackgroundImageUpdater,
            IPhotoProvider photoProvider,
            IFileDownloader fileDownloader,
            IFileService fileService,
            IRandom random,
            IEventEmitter eventEmitter,
            FlickrParameters flickrParameters,
            IClock clock,
            FlickrParametersService flickrParametersService)
        {
            _desktopBackgroundImageUpdater = desktopBackgroundImageUpdater;
            _photoProvider = photoProvider;
            _fileDownloader = fileDownloader;
            _fileService = fileService;
            _random = random;
            _eventEmitter = eventEmitter;
            _flickrParameters = flickrParameters;
            _clock = clock;
            _flickrParametersService = flickrParametersService;
        }

        // ----- Public methods

        public async Task Handle(ChangeDesktopBackgroundImageRandomlyCommand command)
        {
            await UpdateDesktopBackgroundImageToRandomPhoto();
        }

        // ----- Internal logic
        private async Task UpdateDesktopBackgroundImageToRandomPhoto()
        {
            try {
                var photoUrl = await SelectRandomPhotoUrl();
                await ChangeBackgroundImageToPhotoUrl(photoUrl);
                await SaveLastUpdateDateToNow();
                _eventEmitter.Emit(new DesktopBackgroundImageUpdated());
            }
            catch {
                await SaveLastUpdateDateToNow();
                _eventEmitter.Emit(new DesktopBackgroundImageUpdateFailed());
                throw;
            }
        }

        private async Task<string> SelectRandomPhotoUrl()
        {
            var photos = await _photoProvider.GetPhotos();
            if (photos.Any() == false) {
                throw new NoPhotoFound(_flickrParameters.UserId, _flickrParameters.Tags);
            }
            return photos.ElementAt(_random.RandomInteger(photos.Count));
        }

        private async Task ChangeBackgroundImageToPhotoUrl(string photoUrl)
        {
            var localFilePath = await _fileDownloader.Download(photoUrl);
            var finalFilePath = Path.Combine(IMAGES_FOLDER, Path.GetFileName(localFilePath));
            if (!Directory.Exists(IMAGES_FOLDER)) {
                Directory.CreateDirectory(IMAGES_FOLDER);
            }
            await this._fileService.Move(localFilePath, finalFilePath);
            await _desktopBackgroundImageUpdater.ChangeBackgroundImage(finalFilePath, PicturePosition.Fit);
        }

        private async Task SaveLastUpdateDateToNow()
        {
            _flickrParameters.BackgroundImageLastRefreshDate = _clock.Now();
            await _flickrParametersService.Save(_flickrParameters);
        }
    }
}