using System;
using System.Threading.Tasks;
using backgroundr.application;
using backgroundr.cqrs;
using backgroundr.domain;
using NSubstitute;
using Xunit;

namespace backgroundr.tests
{
    public class TimerFeature
    {
        private static readonly DateTime SOME_DATE = new DateTime(2018, 12, 07, 12, 0, 0);

        private readonly BackgroundrTimer _timer;
        private readonly IClock _clock;
        private readonly ICommandDispatcher _commandDispatcher;

        public TimerFeature()
        {
            _clock = Substitute.For<IClock>();
            _commandDispatcher = Substitute.For<ICommandDispatcher>();

            _timer = new BackgroundrTimer(
                _clock,
                _commandDispatcher
            );
        }
        
        [Theory]
        [InlineData("00:00:01")]
        [InlineData("00:12:13")]
        [InlineData("12:00:00")]
        public async Task change_background_instantly_if_refresh_period_expired(string refreshPeriod)
        {
            // Arrange
            var nextRefreshDate = SOME_DATE.Add(TimeSpan.Parse(refreshPeriod));
            _clock.Now().Returns(nextRefreshDate);

            // Act
            await _timer.Start(nextRefreshDate);

            // Assert
            await _commandDispatcher
                .Received(1)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Theory]
        [InlineData("00:00:05")]
        [InlineData("00:12:13")]
        [InlineData("12:00:00")]
        public async Task do_not_change_background_instantly_if_within_refresh_period(string refreshPeriod)
        {
            // Data
            var period = TimeSpan.Parse(refreshPeriod);

            // Arrange
            _clock.Now().Returns(SOME_DATE.Add(period.Divide(2)));

            // Act
            await _timer.Start(SOME_DATE.Add(period));

            // Assert
            await _commandDispatcher
                .Received(0)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Fact]
        public async Task change_background_when_refresh_period_ellapsed()
        {
            // Data
            const int timeBeforeChange = 500;
            var nextRefreshDate = SOME_DATE.Add(TimeSpan.FromDays(1));

            // Arrange
            _clock.Now().Returns(nextRefreshDate.Subtract(TimeSpan.FromMilliseconds(timeBeforeChange)));

            // Act
            await _timer.Start(nextRefreshDate);
            await Task.Delay(timeBeforeChange);

            // Assert
            await _commandDispatcher
                .Received(1)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Fact]
        public async Task stopping_timer_do_not_change_background()
        {
            // Data
            const int timeMs = 50;

            // Arrange
            _clock.Now().Returns(SOME_DATE.Subtract(TimeSpan.FromMilliseconds(timeMs)));

            // Act
            await _timer.Start(SOME_DATE);
            await _timer.Stop();
            await Task.Delay(TimeSpan.FromMilliseconds(timeMs + 10));

            // Assert
            await _commandDispatcher
                .Received(0)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }
    }
}
