using UnityEngine;
using UnityEngine.UI;

namespace Core.Utils
{
    public class CameraController : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private Canvas _rootCanvas;
        #endregion

        #region Private Fields

        private CanvasScaler _canvasScaler;
        private Vector2 _referenceResolution;
        private float _defaultHeightSize;
        private Camera _camera;

        private const float DefaultPositionZ = -1;

        private float _defaultAspect;
        private float _defaultWidthSize;
        
        #endregion

        #region Unity Events
        
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _canvasScaler = _rootCanvas.GetComponent<CanvasScaler>();
            _referenceResolution = _canvasScaler.referenceResolution;
            _defaultHeightSize = _camera.orthographicSize;
                
            _defaultAspect = _referenceResolution.x / _referenceResolution.y;
            _defaultWidthSize = _defaultHeightSize * _defaultAspect;
            
            SetCameraPosition();
        }
        
        #endregion

        #region Private Methods
        
        private void SetCameraPosition()
        {
            if (_camera.aspect <= _defaultAspect)
            {
                _canvasScaler.matchWidthOrHeight = 0;

                _camera.orthographicSize = _defaultWidthSize / _camera.aspect;
            }
            else
            {
                _canvasScaler.matchWidthOrHeight = 1;
            }

            var cameraTransform = _camera.transform;
            cameraTransform.position = new Vector3(cameraTransform.position.x, _camera.orthographicSize, DefaultPositionZ);
        }
        #endregion
    }
}
