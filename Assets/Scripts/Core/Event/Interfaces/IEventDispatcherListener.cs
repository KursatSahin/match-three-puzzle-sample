namespace Core.Event
{
    public interface IEventDispatcherListener
    {
        void SubscribeEvents();
        void UnsubscribeEvents();
    }
}