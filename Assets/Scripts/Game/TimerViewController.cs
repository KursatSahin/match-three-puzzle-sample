using Core.Event;
using Core.Service;
using TMPro;
using UnityEngine;
using Utils;

namespace Game
{
    public class TimerViewController : MonoBehaviour
    {
        #region Inspector Fileds
        [SerializeField] private float _totalTime = 60f;
        [SerializeField] private TextMeshProUGUI _remainingTimeText;
        #endregion

        #region Private Fields

        private bool _timerIsRunning = false;
        
        private float _remainingTime;
        private float _endTime;
        private float _startingTime;
        
        private IEventDispatcher _eventDispatcher;
        
        #endregion

        #region Unity Events
        
        private void Start()
        {
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
            _remainingTime = _totalTime;
            Play();
        }

        void Update()
        {
            if (_timerIsRunning)
            {
                _remainingTime = _endTime - Time.realtimeSinceStartup;
                _remainingTimeText.text = $"{(int) Mathf.Clamp(Mathf.Ceil(_remainingTime), 0, _totalTime)}";

                if (_remainingTime <= 0)
                {
                    _timerIsRunning = false;
                    _remainingTimeText.text = "0";
                    _eventDispatcher.Fire(GameEventType.GameEnd);
                }
            }
        }
        
        #endregion

        #region Public Functions
        
        public void SetRemainingTime(float remainingTime)
        {
            _endTime = Time.realtimeSinceStartup + _remainingTime;
        }
        
        public void Play()
        {
            SetRemainingTime(_remainingTime);
            _timerIsRunning = true;
        }
        
        public void Pause()
        {
            _timerIsRunning = false;
        }
        
        #endregion
    }
}
