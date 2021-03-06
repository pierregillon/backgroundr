﻿using System;
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
                    _desktopBackgroundImageUpdater.ChangeBackgroundImage(localFilePath, PicturePosition.Fit);
                }
                else {
                    throw new NoPhotoFound(_flickrParameters.UserId, _flickrParameters.Tags);
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
            _flickrParameters.BackgroundImageLastRefreshDate = _clock.Now();
            _flickrParametersService.Save(_flickrParameters);
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