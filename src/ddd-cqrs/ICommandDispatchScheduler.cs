using System;
using System.Threading.Tasks;

namespace ddd_cqrs
{
    public interface ICommandDispatchScheduler
    {
        Task Schedule<T>(T command, DateTime when) where T : ICommand;
        Task CancelAll();
    }
}