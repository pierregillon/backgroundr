using System;
using System.Threading;
using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;

namespace backgroundr.application
{
    public class BackgroundrTimer
    {
        private readonly BackgroundrParameters _parameters;
        private readonly ICommandDispatcher _commandDispatcher;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;
        private readonly IClock _clock;

        public BackgroundrTimer(
            BackgroundrParameters parameters,
            IClock clock,
            ICommandDispatcher commandDispatcher)
        {
            _parameters = parameters;
            _commandDispatcher = commandDispatcher;
            _clock = clock;
        }

        public async Task Stop()
        {
            if (_cancellationTokenSource != null) {
                _cancellationTokenSource.Cancel();
                try {
                    await _task;
                }
                catch (TaskCanceledException) { }
            }
        }
        public async Task Start()
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
    }
}