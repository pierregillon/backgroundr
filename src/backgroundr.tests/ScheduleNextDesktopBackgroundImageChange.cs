using System;
using System.Threading.Tasks;
using backgroundr.application;
using backgroundr.domain;
using ddd_cqrs;
using NSubstitute;
using Xunit;

namespace backgroundr.tests
{
    public class ScheduleNextDesktopBackgroundImageChangeTests
    {
        private static readonly DateTime SOME_DATE = new DateTime(2019, 04, 07);

        private readonly ScheduleNextDesktopBackgroundImageChangeHandler _handler;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly FlickrParameters _parameters;
        private readonly ICommandDispatchScheduler _commandDispatcherScheduler;

        public ScheduleNextDesktopBackgroundImageChangeTests()
        {
            _parameters = new FlickrParameters();
            _commandDispatcher = Substitute.For<ICommandDispatcher>();
            _commandDispatcherScheduler = Substitute.For<ICommandDispatchScheduler>();
            _handler = new ScheduleNextDesktopBackgroundImageChangeHandler(
                _commandDispatcherScheduler,
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

        [Fact]
        public async Task schedule_at_the_correct_time_the_next_desktop_background_image_update()
        {
            // Arrange
            _parameters.BackgroundImageLastRefreshDate = SOME_DATE;
            _parameters.RefreshPeriod = TimeSpan.FromHours(1);

            // Act
            await _handler.Handle(new ScheduleNextDesktopBackgroundImageChange());

            // Arrange
            await _commandDispatcherScheduler.Received(1).Schedule(Arg.Any<ChangeDesktopBackgroundImageRandomly>(), SOME_DATE.Add(TimeSpan.FromHours(1)));
        }
    }
}
