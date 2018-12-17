using System;
using System.Threading;
using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;

namespace backgroundr.application
{
    public class StartDesktopBackgroundImageTimerHandler : ICommandHandler<StartDesktopBackgroundImageTimer>, IEventListener<DesktopBackgroundChanged>
    {
        private readonly IClock _clock;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly BackgroundrParameters _parameters;
        private Task _task;
        private CancellationTokenSource _cancellationTokenSource;

        public StartDesktopBackgroundImageTimerHandler(
            BackgroundrParameters parameters,
            IClock clock,
            ICommandDispatcher commandDispatcher)
        {
            _parameters = parameters;
            _clock = clock;
            _commandDispatcher = commandDispatcher;
        }

        public async Task Handle(StartDesktopBackgroundImageTimer command)
        {
            if (_parameters.BackgroundImageLastRefreshDate.HasValue) {
                var remainingSeconds = _parameters.BackgroundImageLastRefreshDate.Value.Add(_parameters.RefreshPeriod).Subtract(_clock.Now());
                if (remainingSeconds.TotalSeconds <= 0) {
                    await _commandDispatcher.Dispatch(new ChangeDesktopBackgroundImageRandomly());
                }
                else {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _task = Task.Run(async () => {
                        await Task.Delay(remainingSeconds, _cancellationTokenSource.Token);
                        await _commandDispatcher.Dispatch(new ChangeDesktopBackgroundImageRandomly());
                    }, _cancellationTokenSource.Token);
                }
            }
        }
        public Task On(DesktopBackgroundChanged @event)
        {
            _cancellationTokenSource?.Cancel();
            return Task.Delay(0);
        }
    }
}