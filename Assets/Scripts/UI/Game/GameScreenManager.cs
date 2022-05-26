using Core.Animation.Interfaces;
using Core.Service;
using Game;
using UnityEngine;
using Utils;

namespace UI.Game
{
    public class GameScreenManager : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Game Screen Dependencies")]
        [SerializeField] private Transform _boardPivotPoint;
        [SerializeField] private Transform _gemParentContainer;
        #endregion
    
        #region Private Fields
        private IAnimationManager _animationManager;
        private BoardViewController _boardViewController;
        #endregion

        #region Unity Events

        private void Awake()
        {
        
        }

        private void Start()
        {
            var boardDrawHelper = new BoardDrawHelper(_boardPivotPoint);
            ServiceLocator.Instance.RegisterService<IBoardDrawHelper>(boardDrawHelper);
            _boardViewController = new BoardViewController(_gemParentContainer);
        }

        #endregion
    }
}
