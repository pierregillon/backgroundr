using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;

namespace backgroundr.application
{
    public class RearmTimerListener : IEventListener<DesktopBackgroundChanged>
    {
        private readonly BackgroundrTimer _timer;
        private readonly ICommandDispatcher _commandDispatcher;

        public RearmTimerListener(BackgroundrTimer timer, ICommandDispatcher commandDispatcher)
        {
            _timer = timer;
            _commandDispatcher = commandDispatcher;
        }

        public async Task On(DesktopBackgroundChanged @event)
        {
            await _timer.Stop();
            await _commandDispatcher.Dispatch(new StartDesktopBackgroundImageTimer());
        }
    }
}