using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backgroundr.application;
using backgroundr.cqrs;
using backgroundr.domain;
using backgroundr.infrastructure;
using NSubstitute;
using Xunit;

namespace backgroundr.tests
{
    public class ChangeDesktopBackgroundImageTests
    {
        private static readonly Task<IReadOnlyCollection<string>> NO_IMAGES = Task.FromResult<IReadOnlyCollection<string>>(new string[0]);
        private static readonly Task<IReadOnlyCollection<string>> SOME_IMAGES = Task.FromResult<IReadOnlyCollection<string>>(new[] {
            "https://mywebsite/plane.jpg",
            "https://mywebsite/boat.jpg",
            "https://mywebsite/car.jpg",
        });

        private readonly IDesktopBackgroundImageUpdater _desktopImageBackgroundUpdater;
        private readonly IPhotoProvider _photoProvider;
        private readonly ChangeDesktopBackgroundImageRandomlyHandler _handler;
        private readonly IEventEmitter _eventEmitter;
        private readonly FlickrParameters _flickrParameters;
        private readonly IClock _clock;
        private readonly IFileService _fileService;
        private static readonly DateTime NOW = new DateTime(2018, 12, 26);

        public ChangeDesktopBackgroundImageTests()
        {
            _desktopImageBackgroundUpdater = Substitute.For<IDesktopBackgroundImageUpdater>();
            _photoProvider = Substitute.For<IPhotoProvider>();
            var fileDownloader = Substitute.For<IFileDownloader>();
            fileDownloader.Download(Arg.Any<string>()).Returns(x => x.Arg<string>());
            _eventEmitter = Substitute.For<IEventEmitter>();
            _flickrParameters = new FlickrParameters();
            _clock = Substitute.For<IClock>();
            _fileService = Substitute.For<IFileService>();

            _handler = new ChangeDesktopBackgroundImageRandomlyHandler(
                _desktopImageBackgroundUpdater,
                _photoProvider,
                fileDownloader,
                new PseudoRandom(),
                _eventEmitter,
                _flickrParameters,
                _clock,
                new FlickrParametersService(_fileService)
            );
        }

        [Fact]
        public async Task throw_error_when_no_image_available()
        {
            await Assert.ThrowsAsync<NoPhotoFound>(async () => {
                // Arranges
                _photoProvider
                    .GetPhotos()
                    .Returns(x => NO_IMAGES);

                // Acts
                await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());
            });
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
            _photoProvider
                .GetPhotos()
                .Returns(x => Task.FromResult<IReadOnlyCollection<string>>(availableImages));

            // Acts
            await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());

            // Asserts
            _desktopImageBackgroundUpdater
                .Received(1)
                .ChangeBackgroundImage(Arg.Is<string>(value => availableImages.Contains(value)), Arg.Any<PicturePosition>());
        }

        [Fact]
        public async Task extend_new_background_image()
        {
            // Arrange
            _photoProvider
                .GetPhotos()
                .Returns(x => SOME_IMAGES);

            // Act
            await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());

            // Assert
            _desktopImageBackgroundUpdater
                .Received(1)
                .ChangeBackgroundImage(Arg.Any<string>(), PicturePosition.Fit);
        }

        [Fact(Skip = "appveyor error on multi threads")]
        public async Task do_not_process_concurrent_requests()
        {
            // Arrange
            _photoProvider
                .GetPhotos()
                .Returns(x => SOME_IMAGES)
                .AndDoes(x => {
                    Thread.Sleep(100);
                });

            // Act
            var tasks = new[] {
                Task.Factory.StartNew(async () => {
                    await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());
                }),
                Task.Factory.StartNew(async () => {
                    await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());
                }),
                Task.Factory.StartNew(async () => {
                    await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());
                })
            };

            await Task.WhenAll(tasks);

            // Assert
            _desktopImageBackgroundUpdater
                .Received(1)
                .ChangeBackgroundImage(Arg.Any<string>(), Arg.Any<PicturePosition>());
        }

        [Fact]
        public async Task emit_desktop_background_changed()
        {
            // Arrange
            _photoProvider
                .GetPhotos()
                .Returns(x => SOME_IMAGES);

            // Act
            await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());

            // Assert
            _eventEmitter
                .Received(1)
                .Emit(Arg.Any<DesktopBackgroundImageUpdated>());
        }

        [Fact]
        public async Task save_parameter_last_change_date_to_now()
        {
            // Arrange
            _clock.Now().Returns(NOW);
            _photoProvider
                .GetPhotos()
                .Returns(x => SOME_IMAGES);

            // Act
            await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());

            // Assert
            Assert.Equal(NOW, _flickrParameters.BackgroundImageLastRefreshDate);
        }

        [Fact]
        public async Task update_config_file()
        {
            // Arrange
            _clock.Now().Returns(NOW);
            _photoProvider
                .GetPhotos()
                .Returns(x => SOME_IMAGES);

            // Act
            await _handler.Handle(new ChangeDesktopBackgroundImageRandomly());

            // Assert
            _fileService.Received(1).Serialize(Arg.Any<FlickrParameters>(), Arg.Is<string>(x=>x.EndsWith(".config")));
        }
    }
}