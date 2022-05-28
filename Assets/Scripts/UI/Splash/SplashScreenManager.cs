using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI.Splash
{
    public class SplashScreenManager : MonoBehaviour
    {
        #region Inspector Fiedls

        [Header("Splash Screen Dependencies")] 
        [SerializeField] private TextMeshProUGUI _gameTitle;
        [SerializeField] private Slider _loadingbarSlider;
        [SerializeField] private TextMeshProUGUI _loadingbarSliderText;
    
        #endregion
    
        #region Private Fields
    
        private float _gameTitleAnimDuration = 1.5f;
        private Ease _gameTitleAnimEaseType = Ease.OutBounce;
        private float _loadingbarAnimDuration = 2f;
        #endregion


        private void Awake()
        {
            _gameTitle.gameObject.SetActive(false);
            _loadingbarSlider.gameObject.SetActive(false);
            _loadingbarSlider.value = 0;
            _loadingbarSlider.onValueChanged.AddListener(OnLoadingBarValueChanged);
        }

        private void OnDisable()
        {
            _loadingbarSlider.onValueChanged.RemoveListener(OnLoadingBarValueChanged);
        }

        private void Start()
        {
            GameTitleAnim();
        }

        /// <summary>
        /// Creates a animation tween for the game title and starts
        /// </summary>
        private void GameTitleAnim()
        {
            Sequence sequence = DOTween.Sequence().Pause().SetLink(gameObject);
        
            sequence.OnStart((() =>
            {
                // Set starting position
                var gameTitleStartingPosOffset = ((float) Screen.height / 2) + _gameTitle.rectTransform.rect.height;
                _gameTitle.rectTransform.anchoredPosition = new Vector2(0, gameTitleStartingPosOffset);
                
                // Game title visible
                _gameTitle.gameObject.SetActive(true);
            }));
            
            // Fall game title down
            sequence.Append(_gameTitle.rectTransform.DOAnchorPosY(0f, _gameTitleAnimDuration).SetEase(_gameTitleAnimEaseType));
            
            // Set loading bar visible
            sequence.AppendCallback(() =>
            {
                _loadingbarSlider.gameObject.SetActive(true);
            });
            
            // Loading filling bar animation
            sequence.Append(_loadingbarSlider.DOValue(100, _loadingbarAnimDuration).SetEase(Ease.OutCirc));
            
            // Loading home screen after complete
            sequence.OnComplete(LoadHomeScene);

            sequence.Play();
        }

        /// <summary>
        /// Loading home screen asynchronously
        /// </summary>
        private async void LoadHomeScene()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Strings.Scenes.Home);

            await UniTask.WaitUntil(() => asyncOperation.isDone);
        }
    
        /// <summary>
        /// Display loading percentage on loading bar
        /// </summary>
        /// <param name="sliderValue">Loading percent</param>
        private void OnLoadingBarValueChanged(float sliderValue)
        {
            _loadingbarSliderText.text = $"Loading... {(int)sliderValue}%";
        }
    }
}
