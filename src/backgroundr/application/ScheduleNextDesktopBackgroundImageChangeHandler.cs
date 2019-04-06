using System.Threading.Tasks;
using backgroundr.cqrs;

namespace backgroundr.application
{
    public class ScheduleNextDesktopBackgroundImageChangeHandler : ICommandHandler<ScheduleNextDesktopBackgroundImageChange>
    {
        private readonly CommandDispatchScheduler _commandDispatcherScheduler;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly FlickrParameters _flickrParameters;

        public ScheduleNextDesktopBackgroundImageChangeHandler(
            CommandDispatchScheduler commandDispatcherScheduler,
            ICommandDispatcher commandDispatcher,
            FlickrParameters flickrParameters)
        {
            _commandDispatcherScheduler = commandDispatcherScheduler;
            _commandDispatcher = commandDispatcher;
            _flickrParameters = flickrParameters;
        }

        public async Task Handle(ScheduleNextDesktopBackgroundImageChange command)
        {
            await _commandDispatcherScheduler.CancelAll();
            if (_flickrParameters.BackgroundImageLastRefreshDate.HasValue) {
                var nextRefreshDate = _flickrParameters.BackgroundImageLastRefreshDate.Value.Add(_flickrParameters.RefreshPeriod);
                await _commandDispatcherScheduler.Schedule(new ChangeDesktopBackgroundImageRandomly(), nextRefreshDate);
            }
            else {
                await _commandDispatcher.Dispatch(new ChangeDesktopBackgroundImageRandomly());
            }
        }
    }
}