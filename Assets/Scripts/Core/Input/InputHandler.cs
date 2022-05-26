using System;
using Core.Event;
using Core.Service;
using Core.Utils;
using Game.Gem;
using Lean.Touch;
using UnityEngine;

namespace Core.Input
{
    public class InputHandler : MonoBehaviour
    {
        public static event Action<GemView> Tap;
        public static event Action<GemView, Directions> Swipe;
        
        private Camera _mainCamera;

        private bool _isBlocked = false;
        
        private IEventDispatcher _eventDispatcher;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
        }

        private void OnEnable()
        {
            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerSwipe += OnFingerSwipe;
            _eventDispatcher.Subscribe(GameEventType.BlockInputHandler, OnBlockInputHandler);
            _eventDispatcher.Subscribe(GameEventType.UnblockInputHandler, OnUnblockInputHandler);
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerSwipe -= OnFingerSwipe;
            _eventDispatcher.Unsubscribe(GameEventType.BlockInputHandler, OnBlockInputHandler);
            _eventDispatcher.Unsubscribe(GameEventType.UnblockInputHandler, OnUnblockInputHandler);
        }

        private void OnFingerSwipe(LeanFinger finger)
        {
            if (_isBlocked) return;
            
            var gemView = GetGem(finger);
            if (gemView == null) return;
            
            var swipe = finger.SwipeScreenDelta;
            if (swipe.x < -Mathf.Abs(swipe.y)) Swipe?.Invoke(gemView, Directions.Left);
            if (swipe.x > Mathf.Abs(swipe.y)) Swipe?.Invoke(gemView, Directions.Right);
            if (swipe.y < -Mathf.Abs(swipe.x)) Swipe?.Invoke(gemView, Directions.Down);
            if (swipe.y > Mathf.Abs(swipe.x)) Swipe?.Invoke(gemView, Directions.Up);
        }

        private void OnFingerTap(LeanFinger finger)
        {
            if (_isBlocked) return;
            
            GemView gemView = GetGem(finger);
            if (gemView == null) return;

            Tap?.Invoke(gemView);
        }
        
        private GemView GetGem(LeanFinger finger)
        {
            var hit = Physics2D.Raycast(_mainCamera.ScreenPointToRay(finger.StartScreenPosition).origin, Vector2.zero);

            if (hit.transform != null && hit.transform.gameObject.HasComponent<GemView>())
            {
                return hit.transform.GetComponent<GemView>();
            }

            return null;
        }
        
        private void OnUnblockInputHandler(IEvent e)
        {
            _isBlocked = false;
        }

        private void OnBlockInputHandler(IEvent e)
        {
            _isBlocked = true;
        }
    }
    
    public enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }
}