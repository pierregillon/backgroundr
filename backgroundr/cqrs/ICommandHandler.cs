using System.Threading.Tasks;

namespace backgroundr.cqrs
{
    public interface ICommandHandler<in T>
    {
        Task Handle(T command);
    }

    public interface ICommandHandler<in TCommand, TResult>
    {
        Task<TResult> Handle(TCommand command);
    }
}