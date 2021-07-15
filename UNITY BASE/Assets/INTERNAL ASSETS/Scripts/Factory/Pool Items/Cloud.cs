using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class Cloud : PoolItem
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private float _inverseScalePercent = 40f;

        private RectTransform _leftAnchor;
        private RectTransform _rightAnchor;
        private CloudController.CloudData _data;
        private Vector3 _direction;
        private float _halfWidth;

        //==================================================
        // Methods
        //==================================================

        public override void Activate(params object[] args)
        {
            base.Activate(args);

            CloudController.CloudData data = (CloudController.CloudData)args[0];
            Orientations direction = (Orientations)args[1];
            RectTransform leftPivot = (RectTransform)args[2];
            RectTransform rightPivot = (RectTransform)args[3];

            _leftAnchor = leftPivot;
            _rightAnchor = rightPivot;

            _direction = direction == Orientations.Left ? Vector3.left : Vector3.right;
            _data = data;

            _halfWidth = this.CachedTransform.rect.width * data.scale * GameConstants.HALF_FACTOR;

            Vector3 scale = this.CachedTransform.localScale;
            scale.x *= Random.Range(0f, 100f) <= _inverseScalePercent ? -1f : 1f;
            this.CachedTransform.localScale = scale;
            
            SetActive(true);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            this.Pool.Put(this.gameObject);
        }

        protected override void Subscribe(EventManager.Subscribes subscribe)
        {
            base.Subscribe(subscribe);

            _eventsManager.RefreshEventListener(GameEvents.ResetGameSceneObjects.ToString(), OnResetGame, subscribe);
        }

        private void Update()
        {
            if (this.ActiveSelf)
            {
                this.CachedTransform.localPosition += _direction * _data.speed * Time.deltaTime;

                if (_direction == Vector3.right && this.CachedTransform.localPosition.x > _rightAnchor.localPosition.x + _halfWidth)
                    Deactivate();

                if (_direction == Vector3.left && this.CachedTransform.localPosition.x < _leftAnchor.localPosition.x - _halfWidth)
                    Deactivate();
            }
        }

        #region Events
        private void OnResetGame(object[] args)
        {
            Deactivate();
        }
        #endregion
    }
}