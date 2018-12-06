using System.Collections.Generic;
using System.Threading.Tasks;
using backgroundr.application;
using backgroundr.domain;
using backgroundr.infrastructure;
using NSubstitute;
using NSubstitute.Core;
using Xunit;

namespace backgroundr.tests
{
    public class ChangeDesktopBackground
    {
        private readonly IDesktopBackgroundImageUpdater _desktopImageBackgroundUpdater;
        private readonly IImageProvider _imageProvider;
        private readonly IFileDownloader _fileDownloader;
        private readonly ChangeDesktopBackgroundImageRandomlyHandler _handler;

        public ChangeDesktopBackground()
        {
            _desktopImageBackgroundUpdater = Substitute.For<IDesktopBackgroundImageUpdater>();
            _imageProvider = Substitute.For<IImageProvider>();
            _fileDownloader = Substitute.For<IFileDownloader>();
            _fileDownloader.Download(Arg.Any<string>()).Returns(x => x.Arg<string>());

            _handler = new ChangeDesktopBackgroundImageRandomlyHandler(
                _desktopImageBackgroundUpdater,
                _imageProvider,
                _fileDownloader,
                new PseudoRandom()
            );
        }

        [Fact]
        public async Task do_not_change_background_if_no_image_available()
        {
            // Arranges
            _imageProvider
                .GetImageUrls()
                .Returns(x => (object) Task.FromResult<IReadOnlyCollection<string>>(new string[0]));

            // Acts
            await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());

            // Asserts
            _desktopImageBackgroundUpdater
                .Received(0)
                .ChangeBackgroundImage(Arg.Any<string>(), Arg.Any<PicturePosition>());
        }
    }
}