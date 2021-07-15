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

        protected GameManager _manager;
        protected EventManager _eventManager;
        protected AudioManager _audio;
        
        private GameObject _gameObject;
        private bool _wasInitialized;
        
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

        protected virtual void Subscribe(EventManager.Subscribes subscribe) { }

        public void Initialize()
        {
            if (!_wasInitialized)
            {
                _wasInitialized = true;
                Init();                
            }
        }

        protected virtual void Init()
        {
            _manager = GameManager.Instance;
            _eventManager = EventManager.Instance;
            _audio = AudioManager.Instance;
        }
                     
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