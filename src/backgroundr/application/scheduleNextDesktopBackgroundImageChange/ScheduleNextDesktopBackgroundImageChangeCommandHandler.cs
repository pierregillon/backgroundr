using System;
using System.Threading.Tasks;
using backgroundr.application.changeDesktopBackgroundImageRandomly;
using backgroundr.domain;
using ddd_cqrs;

namespace backgroundr.application.scheduleNextDesktopBackgroundImageChange
{
    public class ScheduleNextDesktopBackgroundImageChangeCommandHandler : ICommandHandler<ScheduleNextDesktopBackgroundImageChangeCommand>
    {
        private readonly ICommandDispatchScheduler _commandDispatcherScheduler;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly FlickrParameters _flickrParameters;
        private readonly ILogger _logger;

        public ScheduleNextDesktopBackgroundImageChangeCommandHandler(
            ICommandDispatchScheduler commandDispatcherScheduler,
            ICommandDispatcher commandDispatcher,
            FlickrParameters flickrParameters,
            ILogger logger)
        {
            _commandDispatcherScheduler = commandDispatcherScheduler;
            _commandDispatcher = commandDispatcher;
            _flickrParameters = flickrParameters;
            _logger = logger;
        }

        public async Task Handle(ScheduleNextDesktopBackgroundImageChangeCommand command)
        {
            await _commandDispatcherScheduler.CancelAll();

            if (_flickrParameters.RefreshPeriod <= TimeSpan.Zero) {
                return;
            }

            if (_flickrParameters.BackgroundImageLastRefreshDate.HasValue) {
                var nextRefreshDate = _flickrParameters.BackgroundImageLastRefreshDate.Value.Add(_flickrParameters.RefreshPeriod);
                _logger.Log($"Next background image change planned on {nextRefreshDate}");
                await _commandDispatcherScheduler.Schedule(new ChangeDesktopBackgroundImageRandomlyCommand(), nextRefreshDate);
            }
            else {
                await _commandDispatcher.Dispatch(new ChangeDesktopBackgroundImageRandomlyCommand());
            }
        }
    }
}