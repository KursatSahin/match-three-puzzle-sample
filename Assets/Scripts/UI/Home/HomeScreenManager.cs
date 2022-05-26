using Core.View;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI.Home
{
    public class HomeScreenManager : BaseMediator
    {
        #region Inspector Fields
        [Header("Home Screen Dependencies")]
        [Header("Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;
    
        [Header("Button Texts")]
        [SerializeField] private TextMeshProUGUI _playButtonText;
        [SerializeField] private TextMeshProUGUI _exitButtonText;
    
        [Header("Titles")]
        [SerializeField] private TextMeshProUGUI _gameTitle;
        #endregion

        #region Private Fields

    
        #endregion

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            _exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        private void OnExitButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        private void OnPlayButtonClicked()
        {
            LoadLoadingScene();
        }
    
        private async void LoadLoadingScene()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Strings.Scenes.Loading);
            await UniTask.WaitUntil(() => asyncOperation.isDone);
        }
    }
}
