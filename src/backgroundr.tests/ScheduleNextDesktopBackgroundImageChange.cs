using System.Threading.Tasks;
using backgroundr.application;
using backgroundr.cqrs;
using backgroundr.domain;
using NSubstitute;
using Xunit;

namespace backgroundr.tests
{
    public class ScheduleNextDesktopBackgroundImageChangeTests
    {
        private readonly ScheduleNextDesktopBackgroundImageChangeHandler _handler;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly FlickrParameters _parameters;
        private IClock _clock;

        public ScheduleNextDesktopBackgroundImageChangeTests()
        {
            _parameters = new FlickrParameters();
            _clock = Substitute.For<IClock>();
            _commandDispatcher = Substitute.For<ICommandDispatcher>();
            _handler = new ScheduleNextDesktopBackgroundImageChangeHandler(
                new CommandDispatchScheduler(_clock, _commandDispatcher, Substitute.For<IFileService>()),
                _commandDispatcher,
                _parameters
            );
        }

        [Fact]
        public async Task change_desktop_background_image_instantly_when_first_time()
        {
            // Arrange
            _parameters.BackgroundImageLastRefreshDate = null;

            // Act
            await _handler.Handle(new ScheduleNextDesktopBackgroundImageChange());

            // Arrange
            await _commandDispatcher.Received(1).Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }
    }
}
