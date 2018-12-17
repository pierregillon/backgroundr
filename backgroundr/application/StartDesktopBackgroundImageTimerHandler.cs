using System;
using System.Threading.Tasks;
using backgroundr.cqrs;

namespace backgroundr.application
{
    public class StartDesktopBackgroundImageTimerHandler : ICommandHandler<StartDesktopBackgroundImageTimer>
    {
        private readonly BackgroundrTimer _timer;
        private readonly BackgroundrParameters _parameters;

        public StartDesktopBackgroundImageTimerHandler(
            BackgroundrTimer timer,
            BackgroundrParameters parameters)
        {
            _timer = timer;
            _parameters = parameters;
        }

        public async Task Handle(StartDesktopBackgroundImageTimer command)
        {
            if (_parameters.BackgroundImageLastRefreshDate.HasValue) {
                var nextRefreshDate = _parameters.BackgroundImageLastRefreshDate.Value.Add(_parameters.RefreshPeriod);
                await _timer.Start(nextRefreshDate);
            }
        }
    }
}