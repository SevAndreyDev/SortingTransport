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