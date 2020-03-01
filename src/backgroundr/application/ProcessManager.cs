using System.Threading.Tasks;
using backgroundr.domain;
using ddd_cqrs;

namespace backgroundr.application
{
    public class ProcessManager :
        IEventListener<DesktopBackgroundImageUpdated>,
        IEventListener<DesktopBackgroundImageUpdateFailed>,
        IEventListener<FlickrConfigurationFileChanged>,
        IEventListener<FileConfigurationReloaded>
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public ProcessManager(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public async Task On(DesktopBackgroundImageUpdated @event)
        {
            await _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());
        }

        public async Task On(DesktopBackgroundImageUpdateFailed @event)
        {
            await _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());
        }

        public async Task On(FlickrConfigurationFileChanged @event)
        {
            await _commandDispatcher.Dispatch(new ReloadFileConfigurationCommand());
        }

        public async Task On(FileConfigurationReloaded @event)
        {
            await _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());
        }
    }
}