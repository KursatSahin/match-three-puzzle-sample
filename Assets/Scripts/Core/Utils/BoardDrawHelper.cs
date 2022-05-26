using Core.Service.Interfaces;
using UnityEngine;

namespace Utils
{
    public class BoardDrawHelper : IBoardDrawHelper
    {
        public static float GemTransformScaleX = .92f;
        public static float GemTransformScaleY = .92f;

        private Vector3 _boardPivot;

        public BoardDrawHelper(Transform boardPivot)
        {
            _boardPivot = boardPivot.position;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(_boardPivot.x + GemTransformScaleX * x, _boardPivot.y + GemTransformScaleY * y);
        }

        public void Initialize()
        {
            // no need to implement
        }
    }
    
    public interface IBoardDrawHelper : IService
    {
        public Vector3 GetWorldPosition(int x, int y);
    }
}