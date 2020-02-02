using System.Threading.Tasks;

namespace ddd_cqrs
{
    public interface IEventListener<in T>
    {
        Task On(T @event);
    }
}