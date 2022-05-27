using System.Collections.Generic;
using Common;
using Containers;
using Core.Animation.Interfaces;
using Core.Service;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using Utils;
using static Containers.ContainerFacade;

namespace Game.Gem
{
    public class GemView : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private GemContainer _gemContainer;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _selectedColor;
        #endregion

        #region Private Fields
        private SpriteRenderer _spriteRenderer;
        private static IBoardDrawHelper _boardDrawHelper;
        private IAnimationManager _animationManager;
        #endregion

        #region Properties
        public static GemView PreviousSelected { get; private set; }
        public GemData Data { get; private set; }
        #endregion
    
        #region Unity Events
        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    
        #endregion

        public void SetGemData (GemData data)
        {
            if (_boardDrawHelper == null)
            {
                _boardDrawHelper = ServiceLocator.Instance.Get<IBoardDrawHelper>();
            }

            if (_animationManager == null)
            {
                _animationManager = ServiceLocator.Instance.Get<IAnimationManager>();
            }

            Data = data;
            _spriteRenderer.sprite = _gemContainer.Gems[(int)Data.Color];
            transform.position = _boardDrawHelper.GetWorldPosition(data.Position.x, data.Position.y);
            
            Data.PositionChanged += OnPositionChanged;
            Data.DestroyGem += OnDestroyGem;
        }

        private void OnDestroyGem()
        {
            Sequence destroySequence = DOTween.Sequence().Pause();
            destroySequence.Append(_spriteRenderer.DOFade(0, 0.4f));
            destroySequence.OnComplete(() =>
            {
                Data.PositionChanged -= OnPositionChanged;
                Data.DestroyGem -= OnDestroyGem;

                Data = null;
                
                LeanPool.Despawn(gameObject);
            });
            
            _animationManager.Enqueue(AnimGroup.Destroy, destroySequence);
        }

        private void OnPositionChanged(Point position)
        {
            Sequence positionChangedSequence = DOTween.Sequence().Pause().SetLink(gameObject); 
            positionChangedSequence.Append(transform.DOMove(_boardDrawHelper.GetWorldPosition(position.x, position.y), 0.4f).SetEase(Ease.OutCirc));

            if (Data.IsSwapped)
            {
                _animationManager.Enqueue(AnimGroup.Gravity, positionChangedSequence);
            }
            else
            {
                _animationManager.Enqueue(AnimGroup.Swap, positionChangedSequence);
            }
        }

        public void Select()
        {
            if (PreviousSelected)
                PreviousSelected.Deselect();

            PreviousSelected = this;
            
            _spriteRenderer.color = _selectedColor;
        }

        public void Deselect()
        {
            if (PreviousSelected == null) 
                return;

            // Hide outline
            PreviousSelected = null;
            
            _spriteRenderer.color = _defaultColor;
        }

        public List<Point> GetAdjacents()
        {
            var adjacents = new List<Point>();

            if (Data.Position.x > 0)  adjacents.Add(new Point(Data.Position.x - 1, Data.Position.y));
            if (Data.Position.x < BoardSettings.boardWidth - 1)  adjacents.Add(new Point(Data.Position.x + 1, Data.Position.y));
            if (Data.Position.y > 0)  adjacents.Add(new Point(Data.Position.x, Data.Position.y - 1));
            if (Data.Position.y < BoardSettings.boardHeight - 1)  adjacents.Add(new Point(Data.Position.x, Data.Position.y + 1));

            return adjacents;
        }

    }
}
