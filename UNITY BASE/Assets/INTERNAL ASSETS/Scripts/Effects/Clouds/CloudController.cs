using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class CloudController : ViewObject
    {
        [System.Serializable]
        public class CloudData
        {
            public float weight;
            public float scale;
            public float speed;
            [Range(0f, 1f)] public float maxHeightAnchorFactor = 1f;
        }

        //==================================================
        // Fields
        //==================================================

        [Header("Pivots")]
        [SerializeField] private RectTransform _left;
        [SerializeField] private RectTransform _middle;
        [SerializeField] private RectTransform _right;
        [SerializeField] private RectTransform _bottom;
        [SerializeField] private RectTransform _top;

        [Header("Maind Settings")]
        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] private Cloud _prefab;
        [SerializeField] private Orientations _direction;
        [SerializeField] private RectTransform[] _prewarmList;
        [SerializeField] private CloudData[] _data;

        [Header("Tween Animation")]
        [SerializeField] private TweenAnimation _fadeTween;

        [Header("Generation Settings")]
        [SerializeField] private float _minDelay;
        [SerializeField] private float _maxDelay;

        private ExtractorByWeights<CloudData> _extractor;
        private bool _generate;

        //==================================================
        // Properties
        //==================================================

        public bool IsFading { get; private set; }

        //==================================================
        // Methods
        //==================================================

        protected override void Init(params object[] args)
        {
            base.Init();

            _extractor = new ExtractorByWeights<CloudData>();
            
            foreach (CloudData item in _data)
            {
                _extractor.Add(item, item.weight);
            }                        
        }

        public override void Activate(params object[] args)
        {
            base.Activate(args);
        
            this.IsFading = true;

            var secuance = DOTween.Sequence();
            secuance.Append(_canvasGroup.DOFade(1f, _fadeTween.Duration)).SetEase(_fadeTween.Ease);

            secuance.OnComplete(() =>
            {
                this.IsFading = false;
            });

            for (int i = 0; i < _prewarmList.Length; i++)
                CreateCloud(_prewarmList[i]);

            _generate = true;
            StartCoroutine(GenerateCloudsProcess());
        }

        public override void Deactivate()
        {
            base.Deactivate();
                        
            this.IsFading = true;

            var secuance = DOTween.Sequence();
            secuance.Append(_canvasGroup.DOFade(0f, _fadeTween.Duration)).SetEase(_fadeTween.Ease);

            secuance.OnComplete(() =>
            {
                this.IsFading = false;
            });

            _generate = false;
        }

        private IEnumerator GenerateCloudsProcess()
        {
            while (_generate)
            {
                float delay = Random.Range(_minDelay, _maxDelay);
                yield return new WaitForSeconds(delay);

                if (_generate)
                    CreateCloud();
            }
        }

        private void CreateCloud(RectTransform target = null)
        {
            CloudData data = _extractor.Get();

            Cloud cloud = Spawner.Instance.Get(PoolObjectKinds.Cloud) as Cloud;
            cloud.CachedTransform.SetParent(this.CachedTransform);
            cloud.CachedTransform.localScale = Vector3.one * data.scale;
                                    
            if (target == null)
            {
                cloud.CachedTransform.position = _direction == Orientations.Left ? _right.position : _left.position;

                float width = cloud.CachedTransform.rect.width * data.scale;
                float heightProgress = Random.Range(0f, 1f);

                Vector3 position = cloud.CachedTransform.localPosition;

                position.x += (_direction == Orientations.Right ? -1f : 1f) * width * GameConstants.HALF_FACTOR;
                position.y = Mathf.Lerp(_bottom.localPosition.y, _top.localPosition.y, heightProgress * data.maxHeightAnchorFactor);
                cloud.CachedTransform.localPosition = position;
            }
            else
            {
                cloud.CachedTransform.position = target.position;
            }

            cloud.Initialize();
            cloud.Activate(data, _direction, _left, _right);
        }
    }
}