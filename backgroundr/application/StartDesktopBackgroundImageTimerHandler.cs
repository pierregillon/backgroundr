using System.Threading.Tasks;
using backgroundr.cqrs;

namespace backgroundr.application
{
    public class StartDesktopBackgroundImageTimerHandler : ICommandHandler<StartDesktopBackgroundImageTimer>
    {
        private readonly CommandDispatchScheduler _commandDispatcherScheduler;
        private readonly BackgroundrParameters _parameters;

        public StartDesktopBackgroundImageTimerHandler(
            CommandDispatchScheduler commandDispatcherScheduler,
            BackgroundrParameters parameters)
        {
            _commandDispatcherScheduler = commandDispatcherScheduler;
            _parameters = parameters;
        }

        public async Task Handle(StartDesktopBackgroundImageTimer command)
        {
            await _commandDispatcherScheduler.CancelAll();
            if (_parameters.BackgroundImageLastRefreshDate.HasValue) {
                var nextRefreshDate = _parameters.BackgroundImageLastRefreshDate.Value.Add(_parameters.RefreshPeriod);
                await _commandDispatcherScheduler.Schedule(new ChangeDesktopBackgroundImageRandomly(), nextRefreshDate);
            }
        }
    }
}