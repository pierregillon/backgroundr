namespace backgroundr.cqrs {
    public interface IEventEmitter
    {
        void Emit<T>(T @event);
    }
}