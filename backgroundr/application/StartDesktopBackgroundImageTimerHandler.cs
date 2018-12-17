using System;
using System.Threading.Tasks;
using backgroundr.cqrs;

namespace backgroundr.application
{
    public class StartDesktopBackgroundImageTimerHandler : ICommandHandler<StartDesktopBackgroundImageTimer>
    {
        private readonly BackgroundrTimer _timer;

        public StartDesktopBackgroundImageTimerHandler(BackgroundrTimer timer)
        {
            _timer = timer;
        }

        public Task Handle(StartDesktopBackgroundImageTimer command)
        {
            return _timer.Start();
        }
    }
}