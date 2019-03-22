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
        private readonly TimeSpan THREAD_SWITCH_DELAY = TimeSpan.FromMilliseconds(10);

        public TimerFeature()
        {
            _clock = Substitute.For<IClock>();
            _commandDispatcher = Substitute.For<ICommandDispatcher>();

            _timer = new BackgroundrTimer(
                _clock,
                _commandDispatcher
            );
        }
        ~TimerFeature()
        {
            _timer.Stop();
        }
        
        [Theory]
        [InlineData("22/03/2019", "18/03/2019")]
        [InlineData("22/03/2019 19:00:00", "22/03/2019 18:00:00")]
        public async Task ask_new_background_image_instantly_if_time_already_ellapsed(string now, string nextRefreshDate)
        {
            // Arrange
            _clock.Now().Returns(DateTime.Parse(now));

            // Act
            await _timer.Start(DateTime.Parse(nextRefreshDate));

            // Assert
            await _commandDispatcher
                .Received(1)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Theory]
        [InlineData("22/03/2019 19:00:00", "22/03/2019 19:00:00.1")]
        public async Task ask_new_background_image_when_time_ellapsed(string now, string nextRefreshDate)
        {
            // Arrange
            _clock.Now().Returns(DateTime.Parse(now));

            // Act
            await _timer.Start(DateTime.Parse(nextRefreshDate));
            var timeDifference = TimeDifference(now, nextRefreshDate);
            await Task.Delay(timeDifference);

            // Assert
            await _commandDispatcher
                .Received(1)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Theory]
        [InlineData("22/03/2019", "23/03/2019")]
        [InlineData("22/03/2019 19:00:00", "22/03/2019 20:00:00")]
        public async Task do_not_ask_for_new_background_image_instantly_if_time_not_ellapsed(string now, string nextRefreshDate)
        {
            // Arrange
            _clock.Now().Returns(DateTime.Parse(now));

            // Act
            await _timer.Start(DateTime.Parse(nextRefreshDate));

            // Assert
            await _commandDispatcher
                .Received(0)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Theory]
        [InlineData("22/03/2019 19:00:00", "22/03/2019 19:00:00.1")]
        public async Task stopping_timer_do_not_ask_new_background_image(string now, string nextRefreshDate)
        {
            // Arrange
            _clock.Now().Returns(DateTime.Parse(now));

            // Act
            await _timer.Start(DateTime.Parse(nextRefreshDate));
            await _timer.Stop();
            await Task.Delay(TimeDifference(now, nextRefreshDate));

            // Assert
            await _commandDispatcher
                .Received(0)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        // ----- Utils

        private TimeSpan TimeDifference(string date1, string date2)
        {
            return DateTime.Parse(date2).Subtract(DateTime.Parse(date1)).Add(THREAD_SWITCH_DELAY);
        }
    }
}
