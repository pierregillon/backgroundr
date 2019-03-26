using System.Threading.Tasks;

namespace backgroundr.cqrs
{
    public interface ICommandDispatcher
    {
        Task Dispatch<T>(T command) where T : ICommand;
        Task<TResult> Dispatch<TCommand, TResult>(TCommand command) where TCommand : ICommand;
    }
}