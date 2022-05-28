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
        
        /// <summary>
        /// BoardDrawHelper constructor with one parameter -board pivot transform-
        /// </summary>
        /// <param name="boardPivot">Bottom-Left corner of board</param>
        public BoardDrawHelper(Transform boardPivot)
        {
            _boardPivot = boardPivot.position;
        }

        /// <summary>
        /// Calculate world position of gem according to board pivot point and gem scale factor
        /// </summary>
        /// <param name="x">Column number</param>
        /// <param name="y">Row number</param>
        /// <returns>World position of gem based on board</returns>
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(_boardPivot.x + GemTransformScaleX * x, _boardPivot.y + GemTransformScaleY * y);
        }
        
        #endregion
    }
    
    public interface IBoardDrawHelper : IService
    {
        /// <summary>
        /// Calls GetWorldPosition() on all IBoardDrawHelper instances
        /// </summary>
        public Vector3 GetWorldPosition(int x, int y);
    }
}