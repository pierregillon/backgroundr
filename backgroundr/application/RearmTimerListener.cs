using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;

namespace backgroundr.application
{
    public class RearmTimerListener : IEventListener<DesktopBackgroundChanged>
    {
        private readonly BackgroundrTimer _timer;

        public RearmTimerListener(BackgroundrTimer timer)
        {
            _timer = timer;
        }

        public Task On(DesktopBackgroundChanged @event)
        {
            _timer.Stop();
            return _timer.Start();
        }
    }
}