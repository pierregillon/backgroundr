using System;
using System.Linq;
using System.Threading.Tasks;
using StructureMap;

namespace ddd_cqrs
{
    public class StructureMapCommandDispatcher : ICommandDispatcher
    {
        private readonly IContainer _container;

        public StructureMapCommandDispatcher(IContainer container)
        {
            _container = container;
        }

        public async Task Dispatch<T>(T command) where T : ICommand
        {
            var handlers = _container.GetAllInstances<ICommandHandler<T>>();
            if (handlers.Any() == false) {
                throw new Exception("No handlers found");
            }
            foreach (var handler in handlers) {
                await handler.Handle(command);
            }
        }
    }
}