using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backgroundr.application;
using backgroundr.domain;
using backgroundr.infrastructure;
using NSubstitute;
using Xunit;

namespace backgroundr.tests
{
    public class ChangeDesktopBackground
    {
        private static readonly Task<IReadOnlyCollection<string>> NO_IMAGES = Task.FromResult<IReadOnlyCollection<string>>(new string[0]);
        private static readonly Task<IReadOnlyCollection<string>> SOME_IMAGES = Task.FromResult<IReadOnlyCollection<string>>(new[] {
            "https://mywebsite/plane.jpg",
            "https://mywebsite/boat.jpg",
            "https://mywebsite/car.jpg",
        });

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
                .Returns(x => NO_IMAGES);

            // Acts
            await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());

            // Asserts
            _desktopImageBackgroundUpdater
                .Received(0)
                .ChangeBackgroundImage(Arg.Any<string>(), Arg.Any<PicturePosition>());
        }

        [Fact]
        public async Task change_background_image_to_a_new_random_one()
        {
            // Data
            var availableImages = new[] {
                "https://mywebsite/plane.jpg",
                "https://mywebsite/boat.jpg",
                "https://mywebsite/car.jpg",
            };

            // Arranges
            _imageProvider
                .GetImageUrls()
                .Returns(x => Task.FromResult<IReadOnlyCollection<string>>(availableImages));

            // Acts
            await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());

            // Asserts
            _desktopImageBackgroundUpdater
                .Received(1)
                .ChangeBackgroundImage(Arg.Is<string>(value => availableImages.Contains(value)), Arg.Any<PicturePosition>());
        }

        [Fact]
        public async Task fill_new_background_image()
        {
            // Arrange
            _imageProvider
                .GetImageUrls()
                .Returns(x => SOME_IMAGES);

            // Act
            await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());

            // Assert
            _desktopImageBackgroundUpdater
                .Received(1)
                .ChangeBackgroundImage(Arg.Any<string>(), PicturePosition.Fill);
        }

        [Fact]
        public async Task do_not_process_concurrent_requests()
        {
            // Arrange
            _imageProvider
                .GetImageUrls()
                .Returns(x => SOME_IMAGES)
                .AndDoes(x => {
                    Thread.Sleep(50);
                });

            // Act
            var tasks = new[] {
                Task.Run(async () => {
                    await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());
                }),
                Task.Run(async () => {
                    await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());
                }),
                Task.Run(async () => {
                    await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());
                }),
            };

            await Task.WhenAll(tasks);

            // Assert
            _desktopImageBackgroundUpdater
                .Received(1)
                .ChangeBackgroundImage(Arg.Any<string>(), Arg.Any<PicturePosition>());
        }
    }
}