using Core.Service;
using Core.Service.Interfaces;

namespace Core.Event
{
    public delegate void EventAction(IEvent e);

    public interface IEventDispatcher : IService, IInitializeService
    {
        void Subscribe(GameEventType gameEventType, EventAction listener);
        void Unsubscribe(GameEventType gameEventType, EventAction listener);
        void Fire(GameEventType gameEventType, IEvent e = null);
    }
}