using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI.EndGame
{
    public class EndGameScreenManager : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Buttons")]
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _restartButton;
    
        [Header("Button Texts")]
        [SerializeField] private TextMeshProUGUI _homeButtonText;
        [SerializeField] private TextMeshProUGUI _restartButtonText;
    
        [Header("Titles")]
        [SerializeField] private TextMeshProUGUI _gameResultTitle;
        #endregion

        #region Unity Events
        
        private void OnEnable()
        {
            _homeButton.onClick.AddListener(OnHomeButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            Destroy(GameObject.FindGameObjectWithTag("ObjectPooler"));
        }

        private void Start()
        {
            _homeButtonText.text = Strings.InGameUI.HomeButtonText;
            _restartButtonText.text = Strings.InGameUI.RestartButtonText;
            _gameResultTitle.text = Strings.InGameUI.CongratsText;
        }

        private void OnDisable()
        {
            _homeButton.onClick.RemoveListener(OnHomeButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }

        #endregion

        #region Private Functions
        
        private void OnRestartButtonClicked()
        {
            SceneManager.LoadScene(Strings.Scenes.Game);
        }

        private void OnHomeButtonClicked()
        {
            SceneManager.LoadScene(Strings.Scenes.Home);
        }
        
        #endregion
    }
}