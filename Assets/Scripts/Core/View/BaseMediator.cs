using Core.Event;
using Core.Service;
using UnityEngine;

namespace Core.View
{
    public abstract class BaseMediator : MonoBehaviour, IEventDispatcherListener
    {
        protected IEventDispatcher eventDispatcher;
        
        protected virtual void Awake()
        {
            eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
        }
        
        public virtual void SubscribeEvents() { }

        public virtual void UnsubscribeEvents() { }
    }
}