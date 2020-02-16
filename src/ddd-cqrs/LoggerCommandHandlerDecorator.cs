using System.Threading.Tasks;

namespace ddd_cqrs
{
    public class LoggerCommandHandlerDecorator<T> : ICommandHandler<T> where T : ICommand
    {
        private readonly ILogger _logger;
        private readonly ICommandHandler<T> _decoratedHandler;

        public LoggerCommandHandlerDecorator(ILogger logger, ICommandHandler<T> decoratedHandler)
        {
            _logger = logger;
            _decoratedHandler = decoratedHandler;
        }

        public async Task Handle(T command)
        {
            _logger.Log($"[COMMAND]\tHandling {typeof(T).Name}");
            await _decoratedHandler.Handle(command);
        }
    }
}