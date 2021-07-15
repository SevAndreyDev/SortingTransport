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
        protected EventManager _eventsManager;
        protected AudioManager _audio;
        
        private bool _wasInitialized;
        
        //==================================================
        // Properties
        //==================================================

        public RectTransform CachedTransform { get { return _cachedTransform; } }
        public float Width { get { return _cachedTransform.rect.width; }  set { _cachedTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value); } }
        public float Height { get { return _cachedTransform.rect.height; } set { _cachedTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value); } }

        public bool ActiveSelf { get { return gameObject.activeSelf; } }

        //==================================================
        // Methods
        //==================================================
        
        public void Initialize()
        {
            if (!_wasInitialized)
            {
                _wasInitialized = true;
                Init();
            }
        }

        protected virtual void Init(params object[] args)
        {
            _manager = GameManager.Instance;
            _eventsManager = EventManager.Instance;
            _audio = AudioManager.Instance;
        }

        public virtual void Activate(params object[] args)
        {
            Subscribe(EventManager.Subscribes.Subscribe);
        }

        public virtual void Deactivate()
        {
            Subscribe(EventManager.Subscribes.Unscribe);
        }
        
        protected virtual void Subscribe(EventManager.Subscribes subscribe) { }

        public void SetActive(bool activeSelf)
        {
            gameObject.SetActive(activeSelf);
        }
    }
}