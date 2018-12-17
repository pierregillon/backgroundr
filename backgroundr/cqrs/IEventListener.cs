using System.Threading.Tasks;

namespace backgroundr.cqrs
{
    public interface IEventListener<in T>
    {
        Task On(T @event);
    }
}