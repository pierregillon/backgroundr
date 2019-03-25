using System.Threading.Tasks;
using backgroundr.cqrs;

namespace backgroundr.application
{
    public class ScheduleNextDesktopBackgroundImageChangeHandler : ICommandHandler<ScheduleNextDesktopBackgroundImageChange>
    {
        private readonly CommandDispatchScheduler _commandDispatcherScheduler;
        private readonly Parameters _parameters;

        public ScheduleNextDesktopBackgroundImageChangeHandler(
            CommandDispatchScheduler commandDispatcherScheduler,
            Parameters parameters)
        {
            _commandDispatcherScheduler = commandDispatcherScheduler;
            _parameters = parameters;
        }

        public async Task Handle(ScheduleNextDesktopBackgroundImageChange command)
        {
            await _commandDispatcherScheduler.CancelAll();
            if (_parameters.BackgroundImageLastRefreshDate.HasValue) {
                var nextRefreshDate = _parameters.BackgroundImageLastRefreshDate.Value.Add(_parameters.RefreshPeriod);
                await _commandDispatcherScheduler.Schedule(new ChangeDesktopBackgroundImageRandomly(), nextRefreshDate);
            }
        }
    }
}