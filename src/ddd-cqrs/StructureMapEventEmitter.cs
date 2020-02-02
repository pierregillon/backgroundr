using StructureMap;

namespace ddd_cqrs
{
    public class StructureMapEventEmitter : IEventEmitter
    {
        private readonly IContainer _container;

        public StructureMapEventEmitter(IContainer container)
        {
            _container = container;
        }

        public async void Emit<T>(T @event)
        {
            var listeners = _container.GetAllInstances<IEventListener<T>>();
            foreach (var handler in listeners) {
                await handler.On(@event);
            }
        }
    }
}