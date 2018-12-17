﻿using System;
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
        [InlineData("00:00:01")]
        [InlineData("00:12:13")]
        [InlineData("12:00:00")]
        public async Task change_background_instantly_if_refresh_period_expired(string refreshPeriod)
        {
            // Arrange
            _parameters.RefreshPeriod = TimeSpan.Parse(refreshPeriod);
            _parameters.BackgroundImageLastRefreshDate = SOME_DATE;
            _clock.Now().Returns(SOME_DATE.Add(_parameters.RefreshPeriod));

            // Act
            await _handler.Handle(new StartDesktopBackgroundImageTimer());

            // Assert
            await _commandDispatcher
                .Received(1)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Theory]
        [InlineData("00:00:01")]
        [InlineData("00:12:13")]
        [InlineData("12:00:00")]
        public async Task do_not_change_background_instantly_if_within_refresh_period(string refreshPeriod)
        {
            // Data
            _parameters.RefreshPeriod = TimeSpan.Parse(refreshPeriod);
            _parameters.BackgroundImageLastRefreshDate = SOME_DATE;

            // Arrange
            var halfPeriod = _parameters.RefreshPeriod.Divide(2);
            _clock.Now().Returns(SOME_DATE.Add(halfPeriod));

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
        public async Task change_background_when_refresh_period_ellapsed()
        {
            // Data
            const int timeBeforeChange = 500;

            // Arrange
            _parameters.RefreshPeriod = TimeSpan.FromDays(1);
            _parameters.BackgroundImageLastRefreshDate = SOME_DATE;
            var remainingDuration = _parameters.RefreshPeriod.Subtract(TimeSpan.FromMilliseconds(timeBeforeChange));
            _clock.Now().Returns(SOME_DATE.Add(remainingDuration));

            // Act
            await _handler.Handle(new StartDesktopBackgroundImageTimer());
            await Task.Delay(timeBeforeChange);

            // Assert
            await _commandDispatcher
                .Received(1)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }

        [Fact]
        public async Task rearm_timer_when_desktop_changed()
        {
            // Arrange
            _parameters.RefreshPeriod = TimeSpan.FromMilliseconds(100);
            _parameters.BackgroundImageLastRefreshDate = SOME_DATE;
            _clock.Now().Returns(SOME_DATE);

            // Act
            await _handler.Handle(new StartDesktopBackgroundImageTimer());
            await Task.Delay(_parameters.RefreshPeriod.Divide(2));
            await _handler.On(new DesktopBackgroundChanged());
            await Task.Delay(_parameters.RefreshPeriod.Divide(2));
            await _handler.On(new DesktopBackgroundChanged());
            await Task.Delay(_parameters.RefreshPeriod.Divide(2));

            // Assert
            await _commandDispatcher
                .Received(0)
                .Dispatch(Arg.Any<ChangeDesktopBackgroundImageRandomly>());
        }
    }
}
