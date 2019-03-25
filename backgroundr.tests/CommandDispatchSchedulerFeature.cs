using System;
using System.Globalization;
using System.Threading.Tasks;
using backgroundr.application;
using backgroundr.cqrs;
using backgroundr.domain;
using NSubstitute;
using Xunit;

namespace backgroundr.tests
{
    public class CommandDispatchSchedulerFeature
    {
        private readonly IClock _clock;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly TimeSpan THREAD_SWITCH_DELAY = TimeSpan.FromMilliseconds(10);
        private readonly CommandDispatchScheduler _commandDispatchScheduler;

        public CommandDispatchSchedulerFeature()
        {
            _clock = Substitute.For<IClock>();
            _commandDispatcher = Substitute.For<ICommandDispatcher>();
            _commandDispatchScheduler = new CommandDispatchScheduler(_clock, _commandDispatcher);
        }
        ~CommandDispatchSchedulerFeature()
        {
#pragma warning disable 4014
            _commandDispatchScheduler.CancelAll();
#pragma warning restore 4014
        }
        
        [Theory]
        [InlineData("22/03/2019", "18/03/2019")]
        [InlineData("22/03/2019 19:00:00", "22/03/2019 18:00:00")]
        public async Task ask_new_background_image_instantly_if_time_already_ellapsed(string now, string nextRefreshDate)
        {
            // Arrange
            _clock.Now().Returns(TheDate(now));

            // Act
            await _commandDispatchScheduler.Schedule(new ChangeDesktopBackgroundImageRandomly(), TheDate(nextRefreshDate));

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
            _clock.Now().Returns(TheDate(now));

            // Act
            await _commandDispatchScheduler.Schedule(new ChangeDesktopBackgroundImageRandomly(), TheDate(nextRefreshDate));
            await Task.Delay(TimeDifferenceBetween(now, nextRefreshDate));

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
            _clock.Now().Returns(TheDate(now));

            // Act
            await _commandDispatchScheduler.Schedule(new ChangeDesktopBackgroundImageRandomly(), TheDate(nextRefreshDate));

            // Assert
            await _commandDispatcher
                .Received(0)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Theory]
        [InlineData("22/03/2019 19:00:00", "22/03/2019 19:00:00.1")]
        public async Task cancelling_schedule_do_not_ask_new_background_image(string now, string nextRefreshDate)
        {
            // Arrange
            _clock.Now().Returns(TheDate(now));

            // Act
            await _commandDispatchScheduler.Schedule(new ChangeDesktopBackgroundImageRandomly(), TheDate(nextRefreshDate));
            await _commandDispatchScheduler.CancelAll();
            await Task.Delay(TimeDifferenceBetween(now, nextRefreshDate));

            // Assert
            await _commandDispatcher
                .Received(0)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        // ----- Utils

        private TimeSpan TimeDifferenceBetween(string date1, string date2)
        {
            return TheDate(date2).Subtract(TheDate(date1)).Add(THREAD_SWITCH_DELAY);
        }
        private static DateTime TheDate(string dateSr)
        {
            return DateTime.Parse(dateSr, new CultureInfo("FR-fr"));
        }
    }
}
