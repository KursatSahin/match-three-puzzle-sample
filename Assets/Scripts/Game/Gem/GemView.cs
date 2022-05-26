using System;
using Containers;
using Core.Service;
using UnityEngine;
using Utils;

namespace Game.Gem
{
    public class GemView : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private GemContainer _gemContainer;
        #endregion

        #region Private Fields
        private SpriteRenderer _spriteRenderer;
        private static IBoardDrawHelper _boardDrawHelper;
        #endregion
    

        #region Properties
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
        }
    }
}
