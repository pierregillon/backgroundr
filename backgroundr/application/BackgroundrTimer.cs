using System;
using System.Threading;
using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;

namespace backgroundr.application
{
    public class BackgroundrTimer
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;
        private readonly IClock _clock;

        public BackgroundrTimer(
            IClock clock,
            ICommandDispatcher commandDispatcher)
        {
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
        public async Task Start(DateTime nextTickDate)
        {
            var timeBeforeDispatch = (int)nextTickDate.Subtract(_clock.Now()).TotalMilliseconds;
            if (timeBeforeDispatch <= 0) {
                await _commandDispatcher.Dispatch(new ChangeDesktopBackgroundImageRandomly());
            }
            else {
                _cancellationTokenSource = new CancellationTokenSource();
                _task = Task.Run(async () => {
                    await Task.Delay(timeBeforeDispatch, _cancellationTokenSource.Token);
                    await _commandDispatcher.Dispatch(new ChangeDesktopBackgroundImageRandomly());
                }, _cancellationTokenSource.Token);
            }
        }
    }
}