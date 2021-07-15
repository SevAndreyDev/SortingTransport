using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class DeltaPositionByResolution : MonoBehaviour
    {
        [System.Serializable]
        private class ResolutionBlock
        {
            public float resolutionFactor;
            public Vector2 deltaPosition;
        }

        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] private RectTransform _cachedTransform;
        [SerializeField] private Vector2 _targetResolution;
        [SerializeField] private Vector2 _targetPosition;

        [Space]
        [SerializeField] private ResolutionBlock _squarePosition;
        [SerializeField] private ResolutionBlock _rectPosition;

        [Space]
        [SerializeField] private bool _showDebugLogs;

        //==================================================
        // Methods
        //==================================================

        private void Start()
        {
            float factor = ((float)Screen.width) / Screen.height;
            float targetFactor = _targetResolution.x / _targetResolution.y;

            Log(string.Format("Target factor = {0}, Current factor = {1}", targetFactor, factor));

            Vector3 position = _targetPosition;

            if (factor < targetFactor)
            {
                float deltaResolutions = Mathf.Abs(targetFactor - _squarePosition.resolutionFactor);
                float currentFactor = Mathf.Abs(factor - _squarePosition.resolutionFactor);
                float progress = Mathf.Clamp01(currentFactor / deltaResolutions);

                position = Vector2.Lerp(_squarePosition.deltaPosition, _targetPosition, progress);
            }
            else if (factor > targetFactor)
            {
                float deltaResolutions = Mathf.Abs(_rectPosition.resolutionFactor - targetFactor);
                float currentFactor = Mathf.Abs(factor - targetFactor);
                float progress = Mathf.Clamp01(currentFactor / deltaResolutions);

                position = Vector2.Lerp(_targetPosition, _rectPosition.deltaPosition, progress);
            }

            _cachedTransform.anchoredPosition = position;
        }

        private void Log(string text)
        {
            if (_showDebugLogs)
                Debug.Log(text);
        }
    }
}