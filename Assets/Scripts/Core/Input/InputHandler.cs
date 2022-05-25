using System;
using Lean.Touch;
using UnityEngine;
using Utils;

namespace Core.Input
{
    public class InputHandler : MonoBehaviour
    {
        public static event Action<Gem> Tap;
        public static event Action<Gem, Directions> Swipe;
        
        private readonly Tuple<Directions, float>[] _directionInDegrees = new Tuple<Directions, float>[4]{
            new Tuple<Directions, float>(Directions.Up, 0f),
            new Tuple<Directions, float>(Directions.Right, 90f),
            new Tuple<Directions, float>(Directions.Down, 180f),
            new Tuple<Directions, float>(Directions.Left, 270f)
        };
        
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerUpdate += OnFingerUpdate;
        }
        
        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerUpdate -= OnFingerUpdate;
        }

        private void OnFingerUpdate(LeanFinger finger)
        {
            if (finger.SwipeScreenDelta.magnitude * LeanTouch.ScalingFactor > LeanTouch.Instance.SwipeThreshold)
            {
                HandleFingerSwipe(finger, finger.StartScreenPosition, finger.ScreenPosition);
            }
        }

        private void HandleFingerSwipe(LeanFinger finger, Vector2 screenFrom, Vector2 screenTo)
        {
            Directions swipeDirection = Directions.Down;
            Gem gem = null;
            
            var finalDelta = screenTo - screenFrom;

            foreach (var directionTupple in _directionInDegrees)
            {
                if (AngleIsValid(finalDelta, directionTupple.Item2))
                {
                    swipeDirection = directionTupple.Item1;
                    break;
                }
            }
            
            gem = GetGem(finger);
            if (gem == null) return;
            
            Swipe?.Invoke(gem, swipeDirection);
        }

        private bool AngleIsValid(Vector2 vector, float requiredAngle)
        {
            var angle      = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
            var angleDelta = Mathf.DeltaAngle(angle, requiredAngle);

            if (angleDelta < -45f || angleDelta >= 45f)
            {
                return false;
            }
            
            return true;
        }

        private void OnFingerTap(LeanFinger finger)
        {
            Gem gem = GetGem(finger);
            if (gem == null) return;

            Tap?.Invoke(gem);
        }
        
        private Gem GetGem(LeanFinger finger)
        {
            var hit = Physics2D.Raycast(_mainCamera.ScreenPointToRay(finger.StartScreenPosition).origin, Vector2.zero);

            if (hit.transform != null && hit.transform.gameObject.HasComponent<Gem>())
            {
                return hit.transform.GetComponent<Gem>();
            }

            return null;
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