namespace ddd_cqrs
{
    public interface IEventEmitter
    {
        void Emit<T>(T @event);
    }
}