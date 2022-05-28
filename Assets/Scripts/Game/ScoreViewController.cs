using System;
using Core.Event;
using Core.Service;
using TMPro;
using UnityEngine;

namespace Game
{
    public class ScoreViewController : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private TextMeshProUGUI _scoreText;
        #endregion

        #region Private Fields

        private int score;
        
        private IEventDispatcher _eventDispatcher;

        #endregion

        #region Properties
        public int Score
        {
            get => score;
            private set
            {
                score = value;
                _scoreText.text = score.ToString();
            }
        }
        
        #endregion

        #region Unity Events
        
        private void OnEnable()
        {
            // Set service locator and subscribe to events
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
            _eventDispatcher.Subscribe(GameEventType.UpdateScore, OnUpdateScore);
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            _eventDispatcher.Subscribe(GameEventType.UpdateScore, OnUpdateScore);
        }

        #endregion
        
        #region Private Functions
        /// <summary>
        /// Score update event handler
        /// </summary>
        /// <param name="e"></param>
        private void OnUpdateScore(IEvent e)
        {
            ScoreUpdateEvent scoreUpdateEvent = e as ScoreUpdateEvent; 
            
            if (scoreUpdateEvent != null) 
                Score += scoreUpdateEvent.Score;
        }
        
        #endregion
    }
}