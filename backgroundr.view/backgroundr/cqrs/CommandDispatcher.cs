using System;
using System.Linq;
using System.Threading.Tasks;
using StructureMap;

namespace backgroundr.cqrs
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IContainer _container;

        public CommandDispatcher(IContainer container)
        {
            _container = container;
        }

        public async Task Dispatch<T>(T command)
        {
            var handlers = _container.GetAllInstances<ICommandHandler<T>>();
            if (handlers.Any() == false) {
                throw new Exception("No handlers found");
            }
            foreach (var handler in handlers) {
                await handler.Handle(command);
            }
        }

        public async Task<TResult> Dispatch<TCommand, TResult>(TCommand command)
        {
            var handler = _container.GetInstance<ICommandHandler<TCommand, TResult>>();
            if (handler == null) {
                throw new Exception("No handlers found");
            }
            return await handler.Handle(command);
        }
    }
}