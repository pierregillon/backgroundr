using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;

namespace backgroundr.application
{
    public class StartDesktopBackgroundImageTimerHandler : ICommandHandler<StartDesktopBackgroundImageTimer>
    {
        private IClock _clock;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly BackgroundrParameters _parameters;

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
#pragma warning disable 4014
                    Task.Run(async () => {
#pragma warning restore 4014
                        await Task.Delay(remainingSeconds);
                        await _commandDispatcher.Dispatch(new ChangeDesktopBackgroundImageRandomly());
                    });
                }
            }
        }
    }
}