using System;
using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class Star : PoolItem, IPointerDownHandler
    {
        [Serializable]
        private class TweenAnimation
        {
            public float duration;
            public Ease ease;
        }

        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] private TweenAnimation _moveTween;
        [SerializeField] private TweenAnimation _scaleTween;
        [SerializeField] private float _toScaleFactor;

        private RectTransform _target;
        private bool _isMoving;

        //==================================================
        // Properties
        //==================================================

        //==================================================
        // Methods
        //==================================================

        public void Activate(StarsView uiTarget)
        {
            _target = uiTarget.CachedTransform;
        }
                
        public void Deactivate()
        {
            _isMoving = false;
            this.Pool.Put(this.gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isMoving)
            {
                _isMoving = true;
                this.CachedTransform.SetParent(_manager.DragField);

                var sequance = DOTween.Sequence();
                sequance.Append(this.CachedTransform.DOMove(_target.position, _moveTween.duration)).SetEase(_moveTween.ease);
                sequance.Insert(0f, this.CachedTransform.DOScale(_toScaleFactor, _scaleTween.duration)).SetEase(_scaleTween.ease);

                sequance.OnComplete(() =>
                {
                    _manager.Stars++;
                    Deactivate();
                });
            }
        }
    }
}