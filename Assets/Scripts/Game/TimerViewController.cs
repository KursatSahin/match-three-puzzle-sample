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
            
            // Timer starts automatically
            Play();
        }

        void Update()
        {
            // Check if timer is running
            if (_timerIsRunning)
            {
                // We need to use this calculation with -Time.realtimeSinceStartup- because of avoiding sync issues when lose onfocus
                
                // Calculate remaining time
                _remainingTime = _endTime - Time.realtimeSinceStartup;
                
                // Update remaining time text
                _remainingTimeText.text = $"{(int) Mathf.Clamp(Mathf.Ceil(_remainingTime), 0, _totalTime)}";

                if (_remainingTime <= 0)
                {
                    _timerIsRunning = false;
                    _remainingTimeText.text = "0";
                    
                    // Send event to notify that game is finished
                    _eventDispatcher.Fire(GameEventType.GameEnd);
                }
            }
        }
        
        #endregion

        #region Public Functions
        /// <summary>
        /// Finds the end time by adding the remaining time to the current time
        /// </summary>
        /// <param name="remainingTime"></param>
        public void SetRemainingTime(float remainingTime)
        {
            _endTime = Time.realtimeSinceStartup + _remainingTime;
        }
        
        /// <summary>
        /// Play the timer
        /// </summary>
        public void Play()
        {
            SetRemainingTime(_remainingTime);
            _timerIsRunning = true;
        }
        /// <summary>
        /// Stop the timer
        /// </summary>
        public void Pause()
        {
            _timerIsRunning = false;
        }
        
        #endregion
    }
}
