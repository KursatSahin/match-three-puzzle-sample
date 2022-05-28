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
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
            _eventDispatcher.Subscribe(GameEventType.UpdateScore, OnUpdateScore);
        }

        private void OnDisable()
        {
            _eventDispatcher.Subscribe(GameEventType.UpdateScore, OnUpdateScore);
        }

        #endregion
        
        #region Private Functions
        private void OnUpdateScore(IEvent e)
        {
            ScoreUpdateEvent scoreUpdateEvent = e as ScoreUpdateEvent; 
            
            if (scoreUpdateEvent != null) 
                Score += scoreUpdateEvent.Score;
        }
        
        #endregion
    }
}