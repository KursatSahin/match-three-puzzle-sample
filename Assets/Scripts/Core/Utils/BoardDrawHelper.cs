using Core.Service.Interfaces;
using UnityEngine;

namespace Core.Utils
{
    public class BoardDrawHelper : IBoardDrawHelper
    {
        #region Private Fields
        
        private const float GemTransformScaleX = .92f;
        private const float GemTransformScaleY = .92f;

        private Vector3 _boardPivot;

        #endregion

        #region Public Methods
        public BoardDrawHelper(Transform boardPivot)
        {
            _boardPivot = boardPivot.position;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(_boardPivot.x + GemTransformScaleX * x, _boardPivot.y + GemTransformScaleY * y);
        }
        
        #endregion
    }
    
    public interface IBoardDrawHelper : IService
    {
        public Vector3 GetWorldPosition(int x, int y);
    }
}