using System.Threading.Tasks;

namespace ddd_cqrs {
    public class LoggerEventListenerDecorator<T> : IEventListener<T>
    {
        private readonly ILogger _logger;
        private readonly IEventListener<T> _decoratedListener;

        public LoggerEventListenerDecorator(ILogger logger, IEventListener<T> decoratedListener)
        {
            _logger = logger;
            _decoratedListener = decoratedListener;
        }

        public async Task On(T @event)
        {
            _logger.Log($"[ EVENT ]\tHandling {typeof(T).Name}");
            await _decoratedListener.On(@event);
        }
    }
}