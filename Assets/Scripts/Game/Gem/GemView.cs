using System.Collections.Generic;
using Common;
using Containers;
using Core.Service;
using DG.Tweening;
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

        private void OnEnable()
        {
            //Register events
            
        }

        private void OnDisable()
        {
            //Unregister events
        }

        public void SetGemData (GemData data)
        {
            if (_boardDrawHelper == null)
            {
                _boardDrawHelper = ServiceLocator.Instance.Get<IBoardDrawHelper>();
            }
            
            Data = data;
            _spriteRenderer.sprite = _gemContainer.Gems[(int)Data.Color];
            transform.position = _boardDrawHelper.GetWorldPosition(data.Position.x, data.Position.y);
            
            Data.PositionChanged += OnPositionChanged;
        }

        private void OnPositionChanged(Point position)
        {
            Sequence positionChangedSequence = DOTween.Sequence().Pause().SetLink(gameObject); 
            positionChangedSequence.Append(transform.DOMove(_boardDrawHelper.GetWorldPosition(position.x, position.y), 0.3f));
            positionChangedSequence.Play();
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
