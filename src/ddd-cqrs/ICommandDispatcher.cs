using System.Threading.Tasks;

namespace ddd_cqrs
{
    public interface ICommandDispatcher
    {
        Task Dispatch<T>(T command) where T : ICommand;
    }
}