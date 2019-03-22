using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;

namespace backgroundr.application
{
    public class RearmTimerListener : IEventListener<DesktopBackgroundChanged>
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public RearmTimerListener(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public async Task On(DesktopBackgroundChanged @event)
        {
            await _commandDispatcher.Dispatch(new StartDesktopBackgroundImageTimer());
        }
    }
}