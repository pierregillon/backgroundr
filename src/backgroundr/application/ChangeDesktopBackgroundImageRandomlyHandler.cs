using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backgroundr.domain;
using backgroundr.infrastructure;
using ddd_cqrs;

namespace backgroundr.application
{
    public class ChangeDesktopBackgroundImageRandomlyHandler : ICommandHandler<ChangeDesktopBackgroundImageRandomly>
    {
        private readonly IDesktopBackgroundImageUpdater _desktopBackgroundImageUpdater;
        private readonly IPhotoProvider _photoProvider;
        private readonly IFileDownloader _fileDownloader;
        private readonly IRandom _random;
        private readonly IEventEmitter _eventEmitter;
        private readonly FlickrParameters _flickrParameters;
        private readonly IClock _clock;
        private readonly FlickrParametersService _flickrParametersService;

        // ----- Constructor

        public ChangeDesktopBackgroundImageRandomlyHandler(
            IDesktopBackgroundImageUpdater desktopBackgroundImageUpdater,
            IPhotoProvider photoProvider,
            IFileDownloader fileDownloader,
            IRandom random,
            IEventEmitter eventEmitter,
            FlickrParameters flickrParameters,
            IClock clock,
            FlickrParametersService flickrParametersService)
        {
            _desktopBackgroundImageUpdater = desktopBackgroundImageUpdater;
            _photoProvider = photoProvider;
            _fileDownloader = fileDownloader;
            _random = random;
            _eventEmitter = eventEmitter;
            _flickrParameters = flickrParameters;
            _clock = clock;
            _flickrParametersService = flickrParametersService;
        }

        // ----- Public methods

        public async Task Handle(ChangeDesktopBackgroundImageRandomly command)
        {
            await UpdateDesktopBackgroundImageToRandomPhoto();
        }

        // ----- Internal logic
        private async Task UpdateDesktopBackgroundImageToRandomPhoto()
        {
            try {
                var photoUrl = await SelectRandomPhotoUrl();
                await ChangeBackgroundImageToPhotoUrl(photoUrl);
                SaveLastUpdateDateToNow();
                _eventEmitter.Emit(new DesktopBackgroundImageUpdated());
            }
            catch {
                SaveLastUpdateDateToNow();
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
            _desktopBackgroundImageUpdater.ChangeBackgroundImage(localFilePath, PicturePosition.Fit);
        }

        private void SaveLastUpdateDateToNow()
        {
            _flickrParameters.BackgroundImageLastRefreshDate = _clock.Now();
            _flickrParametersService.Save(_flickrParameters);
        }
    }
}