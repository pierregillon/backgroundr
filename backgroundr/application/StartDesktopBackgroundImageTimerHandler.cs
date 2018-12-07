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
                var remainingSeconds = _parameters.BackgroundImageLastRefreshDate.Value.AddDays(1).Subtract(_clock.Now());
                if (remainingSeconds.TotalSeconds <= 0) {
                    await _commandDispatcher.Dispatch(new ChangeDesktopBackgroundImageRandomly());
                }
            }
        }
    }
}