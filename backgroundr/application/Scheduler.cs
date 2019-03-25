using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;

namespace backgroundr.application
{
    public class Scheduler : IEventListener<DesktopBackgroundImageUpdated>
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public Scheduler(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public async Task On(DesktopBackgroundImageUpdated @event)
        {
            await _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());
        }
    }
}