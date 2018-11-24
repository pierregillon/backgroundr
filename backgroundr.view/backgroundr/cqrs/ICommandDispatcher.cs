using System.Threading.Tasks;

namespace backgroundr.cqrs
{
    public interface ICommandDispatcher
    {
        Task Dispatch<T>(T command);
        Task<TResult> Dispatch<TCommand, TResult>(TCommand command);
    }
}