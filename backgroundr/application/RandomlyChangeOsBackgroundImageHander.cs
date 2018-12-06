using System.Linq;
using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;
using backgroundr.infrastructure;

namespace backgroundr.application
{
    public class RandomlyChangeOsBackgroundImageHander : ICommandHandler<RandomlyChangeOsBackgroundImage>
    {
        private readonly IDesktopBackgroundImageUpdater _desktopBackgroundImageUpdater;
        private readonly IImageProvider _imageProvider;
        private readonly IFileDownloader _fileDownloader;
        private readonly IRandom _random;

        public RandomlyChangeOsBackgroundImageHander(
            IDesktopBackgroundImageUpdater desktopBackgroundImageUpdater,
            IImageProvider imageProvider,
            IFileDownloader fileDownloader,
            IRandom random)
        {
            _desktopBackgroundImageUpdater = desktopBackgroundImageUpdater;
            _imageProvider = imageProvider;
            _fileDownloader = fileDownloader;
            _random = random;
        }

        public async Task Handle(RandomlyChangeOsBackgroundImage command)
        {
            var photoUrl = await FindNextImage();
            if (string.IsNullOrEmpty(photoUrl) == false) {
                var localFilePath = await _fileDownloader.Download(photoUrl);
                _desktopBackgroundImageUpdater.ChangeBackgroundImage(localFilePath, PicturePosition.Center);
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
    }
}