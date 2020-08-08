using System.Threading.Tasks;
using backgroundr.domain;
using backgroundr.domain.events;
using ddd_cqrs;
using StructureMap;

namespace backgroundr.application.reloadFileConfiguration
{
    public class ReloadFileConfigurationCommandHandler : ICommandHandler<ReloadFileConfigurationCommand>
    {
        private readonly FlickrParametersService _flickrParametersService;
        private readonly IContainer _container;
        private readonly IEventEmitter _eventEmitter;

        public ReloadFileConfigurationCommandHandler(FlickrParametersService flickrParametersService, IContainer container, IEventEmitter eventEmitter)
        {
            _flickrParametersService = flickrParametersService;
            _container = container;
            _eventEmitter = eventEmitter;
        }

        public async Task Handle(ReloadFileConfigurationCommand command)
        {
            _container.Inject(await _flickrParametersService.Read());
            _eventEmitter.Emit(new FileConfigurationReloaded());
        }
    }
}