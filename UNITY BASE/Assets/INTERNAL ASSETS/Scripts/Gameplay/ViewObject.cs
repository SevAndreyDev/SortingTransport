using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class ViewObject : MonoBehaviour
    {
        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] private RectTransform _cachedTransform;

        private GameObject _gameObject;
        
        //==================================================
        // Properties
        //==================================================

        public RectTransform CachedTransform { get { return _cachedTransform; } }
        public float Width { get { return _cachedTransform.rect.width; }  set { _cachedTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value); } }
        public float Height { get { return _cachedTransform.rect.height; } set { _cachedTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value); } }
        public bool ActiveSelf { get { CheckAndCachedGameObject(); return _gameObject.activeSelf; } }
        
        //==================================================
        // Methods
        //==================================================

        public void SetActive(bool activeSelf)
        {
            CheckAndCachedGameObject();
            _gameObject.SetActive(activeSelf);
        }

        private void CheckAndCachedGameObject()
        {
            if (_gameObject == null)
                _gameObject = this.gameObject;
        }
    }
}