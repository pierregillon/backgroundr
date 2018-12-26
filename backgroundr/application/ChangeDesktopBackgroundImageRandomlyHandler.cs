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
        private static readonly object _locker = new object();

        private readonly IDesktopBackgroundImageUpdater _desktopBackgroundImageUpdater;
        private readonly IImageProvider _imageProvider;
        private readonly IFileDownloader _fileDownloader;
        private readonly IRandom _random;
        private readonly IEventEmitter _eventEmitter;
        private readonly BackgroundrParameters _parameters;
        private readonly IClock _clock;
        private readonly IFileService _fileService;

        public ChangeDesktopBackgroundImageRandomlyHandler(
            IDesktopBackgroundImageUpdater desktopBackgroundImageUpdater, 
            IImageProvider imageProvider, 
            IFileDownloader fileDownloader, 
            IRandom random, 
            IEventEmitter eventEmitter,
            BackgroundrParameters parameters,
            IClock clock,
            IFileService fileService)
        {
            _desktopBackgroundImageUpdater = desktopBackgroundImageUpdater;
            _imageProvider = imageProvider;
            _fileDownloader = fileDownloader;
            _random = random;
            _eventEmitter = eventEmitter;
            _parameters = parameters;
            _clock = clock;
            _fileService = fileService;
        }

        public async Task Handle(ChangeDesktopBackgroundImageRandomly command)
        {
            if (Monitor.TryEnter(_locker)) {
                try {
                    var imageUrl = await FindNextImage();
                    if (string.IsNullOrEmpty(imageUrl) == false) {
                        var localFilePath = await _fileDownloader.Download(imageUrl);
                        _desktopBackgroundImageUpdater.ChangeBackgroundImage(localFilePath, PicturePosition.Fill);
                        SaveLastUpdateDateToNow();
                    }
                }
                finally {
                    Monitor.Exit(_locker);
                }
            }
        }
        private async Task<string> FindNextImage()
        {
            var images = await _imageProvider.GetImageUrls();
            if (images.Any() == false) {
                return null;
            }
            var imageIndex = _random.RandomInteger(images.Count);
            return images.ElementAt(imageIndex);
        }
        private void SaveLastUpdateDateToNow()
        {
            _parameters.BackgroundImageLastRefreshDate = _clock.Now();
            _parameters.RefreshPeriod = TimeSpan.FromSeconds(5);
            _fileService.Serialize(_parameters, ".flickr");
            _eventEmitter.Emit(new DesktopBackgroundChanged());
        }
    }
}