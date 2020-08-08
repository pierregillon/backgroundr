using System.Threading.Tasks;
using backgroundr.application.reloadFileConfiguration;
using backgroundr.application.scheduleNextDesktopBackgroundImageChange;
using backgroundr.domain.events;
using ddd_cqrs;

namespace backgroundr.application
{
    public class ProcessManager :
        IEventListener<DesktopBackgroundImageUpdated>,
        IEventListener<DesktopBackgroundImageUpdateFailed>,
        IEventListener<FileConfigurationModified>,
        IEventListener<FileConfigurationReloaded>
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public ProcessManager(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public async Task On(DesktopBackgroundImageUpdated @event)
        {
            await _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChangeCommand());
        }

        public async Task On(DesktopBackgroundImageUpdateFailed @event)
        {
            await _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChangeCommand());
        }

        public async Task On(FileConfigurationModified @event)
        {
            await _commandDispatcher.Dispatch(new ReloadFileConfigurationCommand());
        }

        public async Task On(FileConfigurationReloaded @event)
        {
            await _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChangeCommand());
        }
    }
}