using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;
using backgroundr.infrastructure;

namespace backgroundr.application
{
    public class ChangeDesktopBackgroundImageRandomlyHandler : ICommandHandler<ChangeDesktopBackgroundImageRandomly>
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly IDesktopBackgroundImageUpdater _desktopBackgroundImageUpdater;
        private readonly IPhotoProvider _photoProvider;
        private readonly IFileDownloader _fileDownloader;
        private readonly IRandom _random;
        private readonly IEventEmitter _eventEmitter;
        private readonly Parameters _parameters;
        private readonly IClock _clock;
        private readonly IFileService _fileService;

        // ----- Constructor

        public ChangeDesktopBackgroundImageRandomlyHandler(
            IDesktopBackgroundImageUpdater desktopBackgroundImageUpdater,
            IPhotoProvider photoProvider,
            IFileDownloader fileDownloader,
            IRandom random,
            IEventEmitter eventEmitter,
            Parameters parameters,
            IClock clock,
            IFileService fileService)
        {
            _desktopBackgroundImageUpdater = desktopBackgroundImageUpdater;
            _photoProvider = photoProvider;
            _fileDownloader = fileDownloader;
            _random = random;
            _eventEmitter = eventEmitter;
            _parameters = parameters;
            _clock = clock;
            _fileService = fileService;
        }

        // ----- Public methods
        public async Task Handle(ChangeDesktopBackgroundImageRandomly command)
        {
            await AssertSingleExecution(async () => {
                await UpdateDesktopBackgroundImageToRandomPhoto();
            });
        }

        // ----- Internal logics
        private async Task UpdateDesktopBackgroundImageToRandomPhoto()
        {
            try {
                var photoUrl = await SelectRandomPhoto();
                if (string.IsNullOrEmpty(photoUrl) == false) {
                    var localFilePath = await _fileDownloader.Download(photoUrl);
                    _desktopBackgroundImageUpdater.ChangeBackgroundImage(localFilePath, PicturePosition.Extend);
                }
            }
            finally {
                // Even if error occurred, we send event that background image updated.
                SaveLastUpdateDateToNow();
                _eventEmitter.Emit(new DesktopBackgroundImageUpdated());
            }
        }
        private async Task<string> SelectRandomPhoto()
        {
            var photos = await _photoProvider.GetPhotos();
            if (photos.Any() == false) {
                return null;
            }
            var photoIndex = _random.RandomInteger(photos.Count);
            return photos.ElementAt(photoIndex);
        }
        private void SaveLastUpdateDateToNow()
        {
            _parameters.BackgroundImageLastRefreshDate = _clock.Now();
            _fileService.Serialize(_parameters, ".flickr");
        }

        // ----- Utils
        private async Task AssertSingleExecution(Func<Task> action)
        {
            if (await _semaphoreSlim.WaitAsync(1)) {
                try {
                    await action();
                }
                finally {
                    _semaphoreSlim.Release();
                }
            }
        }
    }
}