using Core.Event;
using Core.Service;
using UnityEngine;

namespace Bootstrapper
{
    public class ServiceBootstrapper
    {
        private const int TweenersCapacity = 1024;
        private const int SequencesCapacity = 256;
        
        private static readonly ServiceLocator _serviceLocator = ServiceLocator.Instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void AfterAssembliesLoaded()
        {
            DG.Tweening.DOTween.defaultRecyclable = true;
            DG.Tweening.DOTween.SetTweensCapacity(TweenersCapacity, SequencesCapacity);
            
            RegisterEventDispatcher();
        }
        
        private static void RegisterEventDispatcher()
        {
            var eventDispatcher = new EventDispatcher();
            eventDispatcher.Initialize();
            _serviceLocator.RegisterService<IEventDispatcher>(eventDispatcher);
        }
    }
}