using System;
using Common;
using Core.Animation.Interfaces;
using Core.Event;
using Core.Service;
using Core.Utils;
using Game.Gem;
using Lean.Touch;
using UnityEngine;

namespace Core.Input
{
    public class InputHandler : IInputHandler
    {
        #region Static Fileds
        public static event Action<GemView> Tap;
        public static event Action<GemView, Point> Swipe;
        
        #endregion

        #region Private Fields
        private Camera _mainCamera;

        private bool _isBlocked = false;
        
        private IEventDispatcher _eventDispatcher;
        private IAnimationManager _animationManager;
        
        #endregion

        #region Public Functions
        
        public InputHandler()
        {
            _mainCamera = Camera.main;
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
            _animationManager = ServiceLocator.Instance.Get<IAnimationManager>();
            _isBlocked = true;
        }
        
        public void Initialize()
        {
            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerSwipe += OnFingerSwipe;
            _eventDispatcher.Subscribe(GameEventType.BlockInputHandler, OnBlockInputHandler);
            _eventDispatcher.Subscribe(GameEventType.UnblockInputHandler, OnUnblockInputHandler);
            
            _isBlocked = false;
        }
        
        public void TearDown()
        {
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerSwipe -= OnFingerSwipe;
            _eventDispatcher.Unsubscribe(GameEventType.BlockInputHandler, OnBlockInputHandler);
            _eventDispatcher.Unsubscribe(GameEventType.UnblockInputHandler, OnUnblockInputHandler);
        }
        
        #endregion

        #region Private Functions
        /// <summary>
        /// OnFingerSwipe event handler
        /// </summary>
        /// <param name="finger"></param>
        private void OnFingerSwipe(LeanFinger finger)
        {
            if (_isBlocked) return;
            
            var gemView = GetGem(finger);
            if (gemView == null) return;
            
            var swipe = finger.SwipeScreenDelta;
            if (swipe.x < -Mathf.Abs(swipe.y)) Swipe?.Invoke(gemView, Point.Left);
            if (swipe.x > Mathf.Abs(swipe.y)) Swipe?.Invoke(gemView, Point.Right);
            if (swipe.y < -Mathf.Abs(swipe.x)) Swipe?.Invoke(gemView, Point.Down);
            if (swipe.y > Mathf.Abs(swipe.x)) Swipe?.Invoke(gemView, Point.Up);
        }

        /// <summary>
        /// OnFingerTap event handler
        /// </summary>
        /// <param name="finger"></param>
        private void OnFingerTap(LeanFinger finger)
        {
            if (_isBlocked) return;
            
            GemView gemView = GetGem(finger);
            if (gemView == null) return;

            Tap?.Invoke(gemView);
        }
        
        /// <summary>
        /// Get gem view by hit by finger
        /// </summary>
        /// <param name="finger"></param>
        /// <returns></returns>
        private GemView GetGem(LeanFinger finger)
        {
            var hit = Physics2D.Raycast(_mainCamera.ScreenPointToRay(finger.StartScreenPosition).origin, Vector2.zero);

            if (hit.transform != null && hit.transform.gameObject.HasComponent<GemView>())
            {
                return hit.transform.GetComponent<GemView>();
            }

            return null;
        }
        
        /// <summary>
        /// OnUnblockInput event handler
        /// </summary>
        /// <param name="e"></param>
        private void OnUnblockInputHandler(IEvent e)
        {
            _isBlocked = false;
        }

        /// <summary>
        /// OnBlockInput event handler
        /// </summary>
        /// <param name="e"></param>
        private async void OnBlockInputHandler(IEvent e)
        {
            _isBlocked = true;

            await _animationManager.Wait();
        }
        
        #endregion
    }
}
