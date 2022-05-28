using Core.Animation;
using Core.Animation.Interfaces;
using Core.Event;
using Core.Input;
using Core.Service;
using Core.Utils;
using Core.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game;
using Lean.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UI.Game
{
    public class GameScreenManager : BaseMediator
    {
        #region Inspector Fields
        [Header("Game Screen Dependencies")]
        [SerializeField] private Transform _boardPivotPoint;
        [SerializeField] private Transform _gemParentContainer;
        #endregion
    
        #region Private Fields
        private IAnimationManager _animationManager;
        private BoardViewController _boardViewController;
        private IInputHandler _inputHandler;
        #endregion

        #region Unity Events
        
        private void Start()
        {
            // Create ands initialize BoardDrawHelper service, and register it to the ServiceLocator
            var boardDrawHelper = new BoardDrawHelper(_boardPivotPoint);
            ServiceLocator.Instance.RegisterService<IBoardDrawHelper>(boardDrawHelper);

            // Create ands initialize AnimationManager service, and register it to the ServiceLocator
            _animationManager = new AnimationManager();
            ServiceLocator.Instance.RegisterService<IAnimationManager>(_animationManager);
            
            // Create ands initialize InputHandler service, and register it to the ServiceLocator
            _inputHandler = new InputHandler();
            ServiceLocator.Instance.RegisterService<IInputHandler>(_inputHandler);
            _inputHandler.Initialize();
            
            // Create ands initialize BoardViewController
            _boardViewController = new BoardViewController(_gemParentContainer);

            // Subscribe to events
            SubscribeEvents();
        }
        
        private void Update()
        {
            // Run board logic every frame
            _boardViewController.RunLogic();
        }
        
        private void LateUpdate()
        {
            // Run animation manager every frame (after board logic), to make sure if any animations remain in animation queue
            _animationManager.Play();
            _animationManager.Reset();
        }

        private void OnDestroy()
        {
            // Clean up
            _inputHandler.TearDown();
            _boardViewController.TearDown();
            DOTween.KillAll();
            LeanPool.DespawnAll();
        }

        #endregion
        
        #region Public Methods

        public override void SubscribeEvents()
        {
            eventDispatcher.Subscribe(GameEventType.GameEnd, OnGameEnd);
        }

        public override void UnsubscribeEvents()
        {
            eventDispatcher.Unsubscribe(GameEventType.GameEnd, OnGameEnd);
        }

        #endregion
        
        #region Private Methods
        
        private async void OnGameEnd(IEvent e)
        {
            UnsubscribeEvents();
            
            _inputHandler.TearDown();
            _boardViewController.TearDown();

            await UniTask.Delay(1000, false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());

            SceneManager.LoadSceneAsync(Strings.Scenes.EndGame,  LoadSceneMode.Additive);
        }
        
        #endregion
    }
}
