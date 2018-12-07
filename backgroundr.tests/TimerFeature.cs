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

        private readonly StartDesktopBackgroundImageTimerHandler _handler;
        private readonly BackgroundrParameters _parameters;
        private readonly IClock _clock;
        private readonly ICommandDispatcher _commandDispatcher;

        public TimerFeature()
        {
            _parameters = new BackgroundrParameters();
            _clock = Substitute.For<IClock>();
            _commandDispatcher = Substitute.For<ICommandDispatcher>();

            _handler = new StartDesktopBackgroundImageTimerHandler(
                _parameters,
                _clock,
                _commandDispatcher
            );
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public async Task change_background_instantly_if_at_least_1_day_passed_since_last_refresh(int dayPassed)
        {
            // Arrange
            _parameters.BackgroundImageLastRefreshDate = SOME_DATE;
            _clock.Now().Returns(SOME_DATE.AddDays(dayPassed));

            // Act
            await _handler.Handle(new StartDesktopBackgroundImageTimer());

            // Assert
            await _commandDispatcher
                .Received(1)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(23)]
        public async Task do_not_change_background_instantly_if_within_the_day_of_last_refresh(int hourPassed)
        {
            // Arrange
            _parameters.BackgroundImageLastRefreshDate = SOME_DATE;
            _clock.Now().Returns(SOME_DATE.AddHours(hourPassed));

            // Act
            await _handler.Handle(new StartDesktopBackgroundImageTimer());

            // Assert
            await _commandDispatcher
                .Received(0)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Fact]
        public async Task do_not_change_background_image_instantly_if_no_last_refresh_date()
        {
            // Arrange
            _parameters.BackgroundImageLastRefreshDate = null;
            _clock.Now().Returns(SOME_DATE);

            // Act
            await _handler.Handle(new StartDesktopBackgroundImageTimer());

            // Assert
            await _commandDispatcher
                .Received(0)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Fact]
        public async Task change_background_when_1_day_passed()
        {
            // Data
            const int timeBeforeChange = 500;

            // Arrange
            _parameters.BackgroundImageLastRefreshDate = SOME_DATE;
            _clock.Now().Returns(SOME_DATE.Add(TimeSpan.FromDays(1).Subtract(TimeSpan.FromMilliseconds(timeBeforeChange))));

            // Act
            await _handler.Handle(new StartDesktopBackgroundImageTimer());
            await Task.Delay(timeBeforeChange);

            // Assert
            await _commandDispatcher
                .Received(1)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }
    }
}
